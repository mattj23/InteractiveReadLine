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
            var items = new string[] {"item one", "item two", "item three"};
            var history = new List<string>(items);
            var keys = KeyBuilder.Create()
                .Add("item four")
                .Enter().Keys;
            var result = this.Fixture(keys, history, false).ReadLine();

            Assert.Equal(3, history.Count);
            foreach (var item in items)
            {
                Assert.Contains(history, x => x.Contains(item));
            }
        }

        [Fact]
        public void UpdatingHistory_Updates()
        {
            var items = new string[] {"item one", "item two", "item three"};
            var history = new List<string>(items);
            var keys = KeyBuilder.Create()
                .Add("item four")
                .Enter().Keys;
            var result = this.Fixture(keys, history, true).ReadLine();

            Assert.Equal(4, history.Count);
            Assert.Contains(history, x => x.Contains("item four"));
            foreach (var item in items)
            {
                Assert.Contains(history, x => x.Contains(item));
            }
        }

        [Theory]
        [InlineData(1, "hist3")]
        [InlineData(2, "histo2")]
        [InlineData(3, "histor1")]
        [InlineData(4, "history0")]
        public void MovesPrevious_Correctly(int count, string expected)
        {
            var keys = KeyBuilder.Create().UpArrow(count).Enter().Keys;
            var handler = FixtureWithHistory(keys);
            var result = handler.ReadLine();

            Assert.Equal(expected, handler.LineState.Text);
            Assert.Equal(expected.Length, handler.LineState.Cursor);
        }

        [Fact]
        public void MovesPrevious_AtLimit_Correctly()
        {
            var keys = KeyBuilder.Create().UpArrow(7).Enter().Keys;
            var handler = FixtureWithHistory(keys);
            var result = handler.ReadLine();

            Assert.Equal("history0", handler.LineState.Text);
        }

        [Theory]
        [InlineData(3, "hist3")]
        [InlineData(2, "histo2")]
        [InlineData(1, "histor1")]
        public void MovesNext_Correctly(int count, string expected)
        {
            var keys = KeyBuilder.Create()
                .Add("this is a test")      // 'Type' the text into the readline buffer
                .LeftArrow(3)               // move the cursor back three positions
                .UpArrow(7)                 // move all the way to the first history entry
                .DownArrow(count)           // Then forward in the history
                .Enter().Keys;
            var handler = FixtureWithHistory(keys);
            var result = handler.ReadLine();

            Assert.Equal(expected, handler.LineState.Text);
            Assert.Equal(expected.Length, handler.LineState.Cursor);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public void MovesBackAndForward_Correctly_IncludingLimit(int steps)
        {
            var keys = KeyBuilder.Create()
                .Add("this is a test")      // 'Type' the text into the readline buffer
                .LeftArrow(3)               // move the cursor back three positions
                .UpArrow(steps)             // move back in the history a set number of steps
                .DownArrow(steps)           // then forward again the same number of steps
                .Enter().Keys;
            var handler = FixtureWithHistory(keys);
            var result = handler.ReadLine();

            Assert.Equal("this is a test", handler.LineState.Text);
            Assert.Equal(11, handler.LineState.Cursor);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public void MovesBackAndForward_Correctly_WithSecondEdit(int steps)
        {
            var keys = KeyBuilder.Create()
                .Add("this is a test")      // 'Type' the text into the readline buffer
                .LeftArrow(4)               // move the cursor back four positions
                .UpArrow(steps)             // move all the way to the first history entry
                .DownArrow(steps)           // then all the way back to the entered buffer
                .Add("modified ")
                .UpArrow(steps)             // move all the way to the first history entry
                .DownArrow(steps)           // then all the way back to the entered buffer
                .Enter().Keys;
            var handler = FixtureWithHistory(keys);
            var result = handler.ReadLine();

            Assert.Equal("this is a modified test", handler.LineState.Text);
            Assert.Equal(19, handler.LineState.Cursor);
        }

        private InputHandler FixtureWithHistory(ConsoleKeyInfo[] keys, bool update = false)
        {
            var history = new List<string> {"history0", "histor1", "histo2", "hist3"};
            return this.Fixture(keys, history, update);
        }

        private InputHandler Fixture(ConsoleKeyInfo[] keys, List<string> history, bool update=false)
        {
            var console = new TestConsole(500, 200, keys);
            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddKeyBehavior(ConsoleKey.LeftArrow, CommonBehaviors.MoveCursorLeft)
                .AddKeyBehavior(ConsoleKey.RightArrow, CommonBehaviors.MoveCursorRight)
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonBehaviors.HistoryNext)
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonBehaviors.HistoryPrevious);

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