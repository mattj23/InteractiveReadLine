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
            var readLine = new ConsoleReadLine("tst>", fakeConsole);
            
            Assert.Equal("tst>", fakeConsole.GetRow(0));
        }

        [Fact]
        public void ConsoleProvider_ReceivesShortTyping_Correctly()
        {
            var keys = KeyBuilder.Create().Add("test!").Enter().Keys;
            var fakeConsole = new TestConsole(5, 10, keys);
            var readLine = new ConsoleReadLine("tst>", fakeConsole);

            var handler = new InputHandler(readLine);
            var result = handler.ReadLine();

            Assert.Equal("tst>test!", fakeConsole.GetRow(0));
        }
    }
}