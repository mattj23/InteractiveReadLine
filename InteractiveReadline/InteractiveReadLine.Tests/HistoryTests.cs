using System;
using System.Collections.Generic;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class HistoryTests
    {
        [Fact]
        public void NullHistory_DoesntCrash_OnNext()
        {
            var keys = KeyBuilder.Create().DownArrow().Enter().Keys;
            var result = Fixture(keys, null).ReadLine();
        }

        [Fact]
        public void NullHistory_DoesntCrash_OnPrevious()
        {
            var keys = KeyBuilder.Create().UpArrow().Enter().Keys;
            var result = Fixture(keys, null).ReadLine();
        }

        [Fact]
        public void EmptyHistory_DoesntCrash_OnNext()
        {
            var keys = KeyBuilder.Create().DownArrow().Enter().Keys;
            var result = Fixture(keys, new List<string>()).ReadLine();
        }

        [Fact]
        public void EmptyHistory_DoesntCrash_OnPrevious()
        {
            var keys = KeyBuilder.Create().UpArrow().Enter().Keys;
            var result = Fixture(keys, new List<string>()).ReadLine();
        }

        [Fact]
        public void NonUpdatingHistory_DoesntUpdate()
        {
            Assert.False(true);
        }

        [Fact]
        public void UpdatingHistory_Updates()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesPrevious_One_Correctly()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesPrevious_Three_Correctly()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesPrevious_AtLimit_Correctly()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesNext_One_Correctly()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesNext_Three_Correctly()
        {
            Assert.False(true);
        }

        [Fact]
        public void MovesNext_AtLimit_Correctly()
        {
            // Returns to the original line state
            Assert.False(true);
        }

        private InputHandler Fixture(ConsoleKeyInfo[] keys, List<string> history, bool update=false)
        {
            var console = new TestConsole(500, 200, keys);
            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonBehaviors.HistoryPrevious)
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonBehaviors.HistoryNext);

            if (history != null)
            {
                if (update)
                    config.SetUpdatingHistorySource(history);
                else
                    config.SetHistorySource(history);
            }

            return new InputHandler(new ConsoleReadLine(console), config);
        }
    }
}