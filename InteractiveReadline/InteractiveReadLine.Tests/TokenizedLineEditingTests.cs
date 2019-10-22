using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class TokenizedLineEditingTests
    {
        [Fact]
        public void SetCursorToEnd_AssignsCorrectToken()
        {
            var line = SimpleLine();
            line.Last.Cursor = line.Last.Text.Length;

            Assert.Equal(line.Last, line.CursorToken);
        }

        [Fact]
        public void ReplacingTokenText_ShorterText_DoesntLoseCursor()
        {
            var line = SimpleLine();
            line.Last.Cursor = line.Last.Text.Length;
            line.Last.Text = "a";

            Assert.Equal(line.Last, line.CursorToken);
        }

        [Fact]
        public void ShrinkTokenBeforeCursor_MovesCursorCorrectly()
        {
            var line = SimpleLine();
            line.Last.Cursor = 4;
            var edit = line.Last.PreviousNotHidden;
            Assert.Equal("driven", line.CursorToken.Text);
            Assert.Equal("testing", edit.Text);

            edit.Text = "a";

            Assert.Equal(4, line.Last.Cursor);
        }

        [Fact]
        public void ExpandTokenBeforeCursor_MovesCursorCorrectly()
        {
            var line = SimpleLine();
            line.Last.Cursor = 4;
            var edit = line.Last.PreviousNotHidden;
            Assert.Equal("driven", line.CursorToken.Text);
            Assert.Equal("testing", edit.Text);

            edit.Text = "supercalifragalisticexpialadocious";

            Assert.Equal(4, line.Last.Cursor);
        }


        private TokenizedLine SimpleLine()
        {
            var line = new TokenizedLine();
            line.Add("this", false);
            line.Add(" ", true);
            line.Add("is", false);
            line.Add(" ", true);
            line.Add("testing", false);
            line.Add("   ", true);
            line.Add("driven", true);
            return line;
        }
    }
}