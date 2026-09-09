using System;
using System.Diagnostics;
using System.Threading;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class CancellationTests
    {
        [Fact]
        public void ReadKey_WithAnAlreadyCancelledToken_Throws()
        {
            var keys = KeyBuilder.Create().Add("unread").Keys;
            var console = new TestConsole(10, 80, keys);
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                Assert.ThrowsAny<OperationCanceledException>(() => provider.ReadKey(source.Token));
            }
        }

        /// <summary>
        /// Cancellation must not consume a keypress because the caller can read the same input through another
        /// route afterward.
        /// </summary>
        [Fact]
        public void ReadKey_WithAnAlreadyCancelledToken_LeavesTheKeysUnread()
        {
            var keys = KeyBuilder.Create().Add("ab").Enter().Keys;
            var console = new TestConsole(10, 80, keys);
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                Assert.ThrowsAny<OperationCanceledException>(() => provider.ReadKey(source.Token));
            }

            // The complete sequence is still queued, so a subsequent read receives the first key.
            Assert.Equal('a', provider.ReadKey().KeyChar);
        }

        [Fact]
        public void ReadKey_WithoutACancellableToken_ReadsNormally()
        {
            var keys = KeyBuilder.Create().Add("x").Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            Assert.Equal('x', provider.ReadKey(CancellationToken.None).KeyChar);
        }

        /// <summary>
        /// When a token can be canceled, the provider polls until a key is available so that it can observe the
        /// token between polls.
        /// </summary>
        [Fact]
        public void ReadKey_PollsUntilAKeyBecomesAvailable()
        {
            var keys = KeyBuilder.Create().Add("z").Keys;
            var console = new TestConsole(10, 80, keys) {PollsBeforeKeyAvailable = 3};
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                var key = provider.ReadKey(source.Token);

                Assert.Equal('z', key.KeyChar);
                Assert.Equal(0, console.PollsBeforeKeyAvailable);
            }
        }

        [Fact]
        public void ReadKey_CancelledWhileWaiting_Throws()
        {
            // No keys are queued, so the read waits until the token is canceled.
            var console = new TestConsole(10, 80);
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMilliseconds(50));

                var stopwatch = Stopwatch.StartNew();
                Assert.ThrowsAny<OperationCanceledException>(() => provider.ReadKey(source.Token));
                stopwatch.Stop();

                // It waited rather than failing immediately, and it did not wait far longer than asked.
                Assert.True(stopwatch.ElapsedMilliseconds >= 25, $"returned after only {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        [Fact]
        public void Read_WhenCancelled_Throws()
        {
            var provider = new ConsoleReadLine(new TestConsole(10, 80));

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMilliseconds(50));

                Assert.ThrowsAny<OperationCanceledException>(
                    () => provider.Read(ReadLineConfig.Basic, source.Token));
            }
        }

        /// <summary>
        /// A canceled read still disposes its provider, which restores the console instead of leaving Ctrl+C
        /// interception enabled.
        /// </summary>
        [Fact]
        public void Read_WhenCancelled_StillRestoresTheConsole()
        {
            var console = new TestConsole(10, 80) {TreatControlCAsInput = false};
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMilliseconds(50));

                Assert.ThrowsAny<OperationCanceledException>(
                    () => provider.Read(ReadLineConfig.Basic, source.Token));
            }

            Assert.False(console.TreatControlCAsInput);
        }

        [Fact]
        public void Read_WithAnUncancelledToken_CompletesNormally()
        {
            var keys = KeyBuilder.Create().Add("still works").Enter().Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            using (var source = new CancellationTokenSource())
            {
                var result = provider.Read(ReadLineConfig.Basic, source.Token);

                Assert.Equal(ReadLineResultKind.Line, result.Kind);
                Assert.Equal("still works", result.Text);
            }
        }

        [Fact]
        public void Constructor_WhenInputIsRedirected_Throws()
        {
            var console = new TestConsole(10, 80) {InputIsRedirected = true};

            var exception = Assert.Throws<InvalidOperationException>(() => new ConsoleReadLine(console));

            Assert.Contains("redirected", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
