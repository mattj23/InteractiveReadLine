using System;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class CutAndPasteTests
    {
        [Fact]
        public void CutToEnd_ThenPaste_RestoresTheLine()
        {
            var keys = KeyBuilder.Create()
                .Add("hello world")
                .LeftArrow(6)
                .Ctrl(ConsoleKey.K)
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("hello world", Read(keys));
        }

        [Fact]
        public void CutToStart_ThenPaste_RestoresTheLine()
        {
            var keys = KeyBuilder.Create()
                .Add("hello world")
                .LeftArrow(5)
                .Ctrl(ConsoleKey.U)
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("hello world", Read(keys));
        }

        [Fact]
        public void CutPreviousWord_ThenPaste_RestoresTheLine()
        {
            var keys = KeyBuilder.Create()
                .Add("alpha beta")
                .Ctrl(ConsoleKey.W)
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("alpha beta", Read(keys));
        }

        /// <summary>
        /// The cut buffer can move text to a different position instead of only restoring it.
        /// </summary>
        [Fact]
        public void CutToEnd_ThenPasteElsewhere_MovesTheText()
        {
            var keys = KeyBuilder.Create()
                .Add("hello world")
                .LeftArrow(6)          // cursor sits just after "hello"
                .Ctrl(ConsoleKey.K)    // cuts " world", leaving "hello"
                .Ctrl(ConsoleKey.A)    // move to the start of the line
                .Ctrl(ConsoleKey.Y)    // paste there
                .Enter().Keys;

            Assert.Equal(" worldhello", Read(keys));
        }

        /// <summary>
        /// Consecutive cuts accumulate in one buffer. The buffer preserves the text's order on the line, so one
        /// paste restores the complete sequence.
        /// </summary>
        [Fact]
        public void ConsecutiveWordCuts_AccumulateInTheOriginalOrder()
        {
            var keys = KeyBuilder.Create()
                .Add("alpha beta gamma")
                .Ctrl(ConsoleKey.W, 3)
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("alpha beta gamma", Read(keys));
        }

        [Fact]
        public void ConsecutiveCutsToEnd_AccumulateInTheOriginalOrder()
        {
            // Two forward cuts from the same position take the line apart from left to right.
            var keys = KeyBuilder.Create()
                .Add("abcdef")
                .LeftArrow(4)          // cursor after "ab"
                .Ctrl(ConsoleKey.K)    // cuts "cdef"
                .Ctrl(ConsoleKey.K)    // cuts nothing, the run continues
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("abcdef", Read(keys));
        }

        /// <summary>
        /// A key that does not perform a cut ends the run, so the next cut starts a fresh buffer.
        /// </summary>
        [Fact]
        public void AKeyBetweenCuts_StartsANewBuffer()
        {
            var keys = KeyBuilder.Create()
                .Add("alpha beta gamma")
                .Ctrl(ConsoleKey.W)    // cuts "gamma"
                .Ctrl(ConsoleKey.E)    // move to end of line, which is not a cut
                .Ctrl(ConsoleKey.W)    // cuts "beta ", starting the buffer over
                .Ctrl(ConsoleKey.A)    // move to the start
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            // Only the second cut is in the buffer. If the run had continued, "gamma" would appear too.
            var result = Read(keys);
            Assert.DoesNotContain("gamma", result);
            Assert.StartsWith("beta", result);
        }

        [Fact]
        public void Paste_WithNothingCut_DoesNothing()
        {
            var keys = KeyBuilder.Create()
                .Add("untouched")
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("untouched", Read(keys));
        }

        /// <summary>
        /// Pasting does not consume the buffer, so the same text can be placed more than once.
        /// </summary>
        [Fact]
        public void Paste_CanBeRepeated()
        {
            var keys = KeyBuilder.Create()
                .Add("ab")
                .Ctrl(ConsoleKey.U)    // cuts "ab"
                .Ctrl(ConsoleKey.Y)
                .Ctrl(ConsoleKey.Y)
                .Ctrl(ConsoleKey.Y)
                .Enter().Keys;

            Assert.Equal("ababab", Read(keys));
        }

        [Fact]
        public void Paste_InsertsAtTheCursorAndLeavesItAfterTheText()
        {
            var keys = KeyBuilder.Create()
                .Add("XY")
                .Ctrl(ConsoleKey.U)    // cuts "XY", line is now empty
                .Add("ab")
                .LeftArrow()           // cursor between a and b
                .Ctrl(ConsoleKey.Y)    // paste "XY" there
                .Add("!")              // typed at the cursor, which must sit after the pasted text
                .Enter().Keys;

            Assert.Equal("aXY!b", Read(keys));
        }

        /// <summary>
        /// The buffer belongs to one read line operation. A new operation starts with nothing to paste,
        /// even when the same configuration object is reused.
        /// </summary>
        [Fact]
        public void CutBuffer_DoesNotCarryOverToTheNextRead()
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();

            var firstKeys = KeyBuilder.Create().Add("secret").Ctrl(ConsoleKey.U).Enter().Keys;
            new ConsoleReadLine(new TestConsole(10, 80, firstKeys)).ReadLine(config);

            var secondKeys = KeyBuilder.Create().Add("fresh").Ctrl(ConsoleKey.Y).Enter().Keys;
            var second = new ConsoleReadLine(new TestConsole(10, 80, secondKeys)).ReadLine(config);

            Assert.Equal("fresh", second);
        }

        [Fact]
        public void AddCtrlNavKeys_BindsPasteToCtrlY()
        {
            var config = ReadLineConfig.Empty.AddCtrlNavKeys();

            Assert.True(config.KeyBehaviors.ContainsKey(new KeyId(ConsoleKey.Y, true, false, false)));
        }

        private static string Read(ConsoleKeyInfo[] keys)
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();
            return new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);
        }
    }
}
