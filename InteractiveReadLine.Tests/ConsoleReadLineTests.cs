using System;
using InteractiveReadLine.Abstractions;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class ConsoleReadLineTests
    {
        [Fact]
        public void ConsoleProvider_InitializesWithPrompt()
        {
            var fakeConsole = new TestConsole(5, 10);
            var readLine = new ConsoleReadLine(fakeConsole);
            
            Assert.Equal("", fakeConsole.GetRow(0));
        }

        /// <summary>
        /// The ReadLine extension method disposes its provider. A caller who also wraps the provider in a using
        /// block therefore disposes it twice, but only the first call should move the cursor to a new row.
        /// </summary>
        [Fact]
        public void Dispose_CalledTwice_FinishesOnlyOnce()
        {
            var fakeConsole = new TestConsole(10, 40);
            var provider = new ConsoleReadLine(fakeConsole);

            provider.Dispose();
            var rowAfterFirstDispose = fakeConsole.CursorTop;

            provider.Dispose();

            Assert.Equal(rowAfterFirstDispose, fakeConsole.CursorTop);
        }

        [Fact]
        public void SystemConsoleWrapper_IsPubliclyConstructable()
        {
            // IConsole is public, so callers must be able to access its default implementation to decorate it.
            IConsole console = new SystemConsoleWrapper();

            Assert.NotNull(console);
        }

        [Fact]
        public void ConsoleProvider_ReceivesShortTyping_Correctly()
        {
            var keys = KeyBuilder.Create().Add("test!").Enter().Keys;
            var fakeConsole = new TestConsole(5, 10, keys);
            var result = new ConsoleReadLine(fakeConsole).ReadLine();

            Assert.Equal("test!", fakeConsole.GetRow(0));
        }

        [Fact]
        public void ConsoleProvider_ReceivesOverflowTyping_Correctly()
        {
            var keys = KeyBuilder.Create().Add("456789abcd").Enter().Keys;
            var fakeConsole = new TestConsole(5, 10, keys);
            var config = ReadLineConfig.Basic.SetFormatter(CommonFormatters.FixedPrompt("012>"));
            var result = new ConsoleReadLine(fakeConsole).ReadLine(config);

            Assert.Equal("012>456789", fakeConsole.GetRow(0));
            Assert.Equal("abcd", fakeConsole.GetRow(1));
        }

        [Fact]
        public void ConsoleProvider_ReceivesBufferHeightOverflow_Correctly()
        {
            var keys = KeyBuilder.Create()
                .Add(new string('0', 6))
                .Add(new string('1', 10))
                .Add(new string('2', 10))
                .Enter().Keys;
            var fakeConsole = new TestConsole(7, 10, keys) {CursorTop = 5};
            var config = ReadLineConfig.Basic.SetFormatter(CommonFormatters.FixedPrompt("012>"));
            var result = new ConsoleReadLine(fakeConsole).ReadLine(config);

            Assert.Equal("012>000000", fakeConsole.GetRow(2));
            Assert.Equal("1111111111", fakeConsole.GetRow(3));
            Assert.Equal("2222222222", fakeConsole.GetRow(4));
        }
    }
}
