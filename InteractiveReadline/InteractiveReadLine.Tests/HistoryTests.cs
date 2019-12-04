using System;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class HistoryTests
    {
        [Fact]
        public void NullHistory_DoesntCrash_OnNext()
        {
            var keys = KeyBuilder.Create()
                .Add(ConsoleKey.DownArrow, false, false, false);
            var console = new TestConsole(500, 200);
            var config = ReadLineConfig.Basic;


        }
    }
}