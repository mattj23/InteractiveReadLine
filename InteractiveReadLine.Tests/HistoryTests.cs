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

        [Fact]
        public void UpdatingHistory_SkipsEmptyLine()
        {
            var history = new List<string> {"existing entry"};
            var keys = KeyBuilder.Create().Enter().Keys;
            this.Fixture(keys, history, true).ReadLine();

            Assert.Single(history);
        }

        [Fact]
        public void UpdatingHistory_SkipsWhitespaceOnlyLine()
        {
            var history = new List<string> {"existing entry"};
            var keys = KeyBuilder.Create().Add("   ").Enter().Keys;
            this.Fixture(keys, history, true).ReadLine();

            Assert.Single(history);
        }

        [Fact]
        public void UpdatingHistory_SkipsLineMatchingMostRecentEntry()
        {
            var history = new List<string> {"item one", "repeated"};
            var keys = KeyBuilder.Create().Add("repeated").Enter().Keys;
            this.Fixture(keys, history, true).ReadLine();

            Assert.Equal(2, history.Count);
        }

        /// <summary>
        /// Only an immediately repeated line is skipped. A line that appears earlier in the history is still
        /// recorded, which moves it to the most recent position.
        /// </summary>
        [Fact]
        public void UpdatingHistory_KeepsLineMatchingAnOlderEntry()
        {
            var history = new List<string> {"repeated", "something else"};
            var keys = KeyBuilder.Create().Add("repeated").Enter().Keys;
            this.Fixture(keys, history, true).ReadLine();

            Assert.Equal(3, history.Count);
            Assert.Equal("repeated", history[2]);
        }

        /// <summary>
        /// SetUpdatingHistorySource performs the filtering. The handler does not, so a directly supplied
        /// update action still receives every finalized line, including empty lines.
        /// </summary>
        [Fact]
        public void CustomHistoryUpdateAction_ReceivesEveryFinalizedLine()
        {
            var recorded = new List<string>();
            var console = new TestConsole(500, 200, KeyBuilder.Create().Enter().Keys);
            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonKeyBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .SetHistoryUpdateAction(recorded.Add);

            new ReadLineHandler(new ConsoleReadLine(console), config).ReadLine();

            Assert.Single(recorded);
            Assert.Equal(string.Empty, recorded[0]);
        }

        [Fact]
        public void SetUpdatingHistorySource_WithNullList_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ReadLineConfig.Empty.SetUpdatingHistorySource(null));
        }

        /// <summary>
        /// The configuration holds a live reference to the history, so the collection can grow after the
        /// handler is constructed and before the user navigates. HistoryNext then steps forward to the
        /// pre-history state, which remains empty because HistoryPrevious was never called to populate it.
        /// </summary>
        [Fact]
        public void HistoryNext_WhenHistoryGrewAfterConstruction_DoesNotThrow()
        {
            var handler = HandlerOverLiveHistory(out var history);
            history.Add("four");

            var exception = Record.Exception(() => handler.HistoryNext());

            Assert.Null(exception);
            Assert.Equal(string.Empty, handler.LineState.Text);
        }

        /// <summary>
        /// The live reference also allows the history to shrink, which can leave the index past the end of the
        /// collection. Both navigation directions would index out of range without clamping.
        /// </summary>
        [Fact]
        public void HistoryNext_WhenHistoryShrankAfterConstruction_DoesNotThrow()
        {
            var handler = HandlerOverLiveHistory(out var history);
            history.RemoveRange(1, 2);

            var exception = Record.Exception(() => handler.HistoryNext());

            Assert.Null(exception);
            Assert.Equal(string.Empty, handler.LineState.Text);
        }

        [Fact]
        public void HistoryPrevious_WhenHistoryShrankAfterConstruction_ShowsMostRecentRemainingEntry()
        {
            var handler = HandlerOverLiveHistory(out var history);
            history.RemoveRange(1, 2);

            var exception = Record.Exception(() => handler.HistoryPrevious());

            Assert.Null(exception);
            Assert.Equal("one", handler.LineState.Text);
        }

        /// <summary>
        /// After the history shrinks, navigating backward and forward again must still restore the text that
        /// the user entered instead of leaving the index stranded.
        /// </summary>
        [Fact]
        public void HistoryNavigation_AfterHistoryShrank_StillRestoresEnteredText()
        {
            var history = new List<string> {"one", "two", "three"};
            var keys = KeyBuilder.Create().Add("typed").UpArrow().DownArrow().Enter().Keys;
            var handler = this.Fixture(keys, history);

            history.RemoveRange(1, 2);
            handler.ReadLine();

            Assert.Equal("typed", handler.LineState.Text);
        }

        private ReadLineHandler HandlerOverLiveHistory(out List<string> history)
        {
            history = new List<string> {"one", "two", "three"};
            var console = new TestConsole(500, 200, new ConsoleKeyInfo[0]);
            var config = ReadLineConfig.Empty.SetHistorySource(history);
            return new ReadLineHandler(new ConsoleReadLine(console), config);
        }

        private ReadLineHandler FixtureWithHistory(ConsoleKeyInfo[] keys, bool update = false)
        {
            var history = new List<string> {"history0", "histor1", "histo2", "hist3"};
            return this.Fixture(keys, history, update);
        }

        private ReadLineHandler Fixture(ConsoleKeyInfo[] keys, List<string> history, bool update=false)
        {
            var console = new TestConsole(500, 200, keys);
            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonKeyBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddKeyBehavior(ConsoleKey.LeftArrow, CommonKeyBehaviors.MoveCursorLeft)
                .AddKeyBehavior(ConsoleKey.RightArrow, CommonKeyBehaviors.MoveCursorRight)
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonKeyBehaviors.HistoryNext)
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonKeyBehaviors.HistoryPrevious);

            if (history != null)
            {
                if (update)
                    config.SetUpdatingHistorySource(history);
                else
                    config.SetHistorySource(history);
            }

            return new ReadLineHandler(new ConsoleReadLine(console), config);
        }
    }
}
