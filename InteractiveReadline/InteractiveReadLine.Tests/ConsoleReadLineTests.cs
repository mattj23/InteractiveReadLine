using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class ConsoleReadLineTests
    {
        [Fact]
        public void ConsoleProvider_ReadsBufferSizesRight()
        {
            var fakeConsole = new TestConsole(5, 10);
            var readLine = new ConsoleReadLine(fakeConsole);
            readLine.SetText("hello");

            Assert.Equal("hello", fakeConsole.GetRow(0));
        }
    }
}