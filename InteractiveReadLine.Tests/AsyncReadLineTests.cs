using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class AsyncReadLineTests
    {
        [Fact]
        public async Task ReadAsync_ProducesALineResult()
        {
            var keys = KeyBuilder.Create().Add("typed asynchronously").Enter().Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            var result = await provider.ReadAsync(ReadLineConfig.Basic);

            Assert.Equal(ReadLineResultKind.Line, result.Kind);
            Assert.Equal("typed asynchronously", result.Text);
        }

        [Fact]
        public async Task ReadLineAsync_ReturnsTheText()
        {
            var keys = KeyBuilder.Create().Add("hello").Enter().Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            Assert.Equal("hello", await provider.ReadLineAsync(ReadLineConfig.Basic));
        }

        /// <summary>
        /// The two loops differ only in how they wait for a key, so the same key sequence must produce the same
        /// result and console output.
        /// </summary>
        [Theory]
        [InlineData("plain text")]
        [InlineData("editing")]
        public async Task ReadAsync_MatchesRead_ForTheSameKeys(string text)
        {
            var syncConsole = new TestConsole(10, 80, Editing(text));
            var asyncConsole = new TestConsole(10, 80, Editing(text));

            var syncResult = new ConsoleReadLine(syncConsole).Read(ReadLineConfig.Basic);
            var asyncResult = await new ConsoleReadLine(asyncConsole).ReadAsync(ReadLineConfig.Basic);

            Assert.Equal(syncResult.Kind, asyncResult.Kind);
            Assert.Equal(syncResult.Text, asyncResult.Text);
            Assert.Equal(syncConsole.BufferToString(), asyncConsole.BufferToString());
        }

        [Fact]
        public async Task ReadAsync_HistoryNavigationBehavesTheSameAsTheSyncLoop()
        {
            var syncHistory = new List<string> {"first", "second"};
            var asyncHistory = new List<string> {"first", "second"};

            var keys = KeyBuilder.Create().UpArrow(2).Enter().Keys;

            var syncResult = new ConsoleReadLine(new TestConsole(10, 80, keys))
                .Read(ReadLineConfig.Basic.SetUpdatingHistorySource(syncHistory));
            var asyncResult = await new ConsoleReadLine(new TestConsole(10, 80, keys))
                .ReadAsync(ReadLineConfig.Basic.SetUpdatingHistorySource(asyncHistory));

            Assert.Equal("first", syncResult.Text);
            Assert.Equal(syncResult.Text, asyncResult.Text);
            Assert.Equal(syncHistory, asyncHistory);
        }

        [Fact]
        public async Task ReadAsync_CtrlC_ProducesACancelledResultRatherThanThrowing()
        {
            var keys = KeyBuilder.Create().Add("abandon me").CtrlC().Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            var result = await provider.ReadAsync(ReadLineConfig.Basic);

            Assert.Equal(ReadLineResultKind.Cancelled, result.Kind);
        }

        [Fact]
        public async Task ReadAsync_CtrlD_OnAnEmptyLine_ProducesAnEndOfInputResult()
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();
            var provider = new ConsoleReadLine(new TestConsole(10, 80, KeyBuilder.Create().CtrlD().Keys));

            var result = await provider.ReadAsync(config);

            Assert.Equal(ReadLineResultKind.EndOfInput, result.Kind);
        }

        [Fact]
        public async Task ReadKeyAsync_WithAnAlreadyCancelledToken_ThrowsWithoutConsumingAKey()
        {
            var keys = KeyBuilder.Create().Add("ab").Keys;
            var provider = new ConsoleReadLine(new TestConsole(10, 80, keys));

            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => provider.ReadKeyAsync(source.Token));
            }

            Assert.Equal('a', provider.ReadKey().KeyChar);
        }

        [Fact]
        public async Task ReadKeyAsync_WaitsUntilAKeyBecomesAvailable()
        {
            var console = new TestConsole(10, 80, KeyBuilder.Create().Add("z").Keys)
            {
                PollsBeforeKeyAvailable = 3
            };
            var provider = new ConsoleReadLine(console);

            var key = await provider.ReadKeyAsync();

            Assert.Equal('z', key.KeyChar);
            Assert.Equal(0, console.PollsBeforeKeyAvailable);
        }

        /// <summary>
        /// The asynchronous path does not occupy a thread while waiting for the user. When no key is available,
        /// the returned task must remain incomplete so that control returns to the caller.
        /// </summary>
        [Fact]
        public async Task ReadKeyAsync_ReturnsAnIncompleteTaskWhileWaiting()
        {
            var provider = new ConsoleReadLine(new TestConsole(10, 80));

            using (var source = new CancellationTokenSource())
            {
                var pending = provider.ReadKeyAsync(source.Token);

                Assert.False(pending.IsCompleted);

                source.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
            }
        }

        [Fact]
        public async Task ReadAsync_WhenCancelled_Throws()
        {
            // No keys are queued, so the read waits until the token is canceled.
            var provider = new ConsoleReadLine(new TestConsole(10, 80));

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMilliseconds(50));

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => provider.ReadAsync(ReadLineConfig.Basic, source.Token));
            }
        }

        /// <summary>
        /// A canceled asynchronous read still disposes its provider, so it restores the console as an ordinary
        /// read does.
        /// </summary>
        [Fact]
        public async Task ReadAsync_WhenCancelled_StillRestoresTheConsole()
        {
            var console = new TestConsole(10, 80) {TreatControlCAsInput = false};
            var provider = new ConsoleReadLine(console);

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMilliseconds(50));

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => provider.ReadAsync(ReadLineConfig.Basic, source.Token));
            }

            Assert.False(console.TreatControlCAsInput);
        }

        [Fact]
        public async Task ReadLineAsync_FlattensCancelledToEmptyAndEndOfInputToNull()
        {
            var cancelled = await new ConsoleReadLine(new TestConsole(10, 80, KeyBuilder.Create().CtrlC().Keys))
                .ReadLineAsync(ReadLineConfig.Basic);

            var ended = await new ConsoleReadLine(new TestConsole(10, 80, KeyBuilder.Create().CtrlD().Keys))
                .ReadLineAsync(ReadLineConfig.Basic.AddCtrlNavKeys());

            Assert.Equal(string.Empty, cancelled);
            Assert.Null(ended);
        }

        private static ConsoleKeyInfo[] Editing(string text) =>
            KeyBuilder.Create().Add(text).LeftArrow(2).Add("XY").Enter().Keys;
    }
}
