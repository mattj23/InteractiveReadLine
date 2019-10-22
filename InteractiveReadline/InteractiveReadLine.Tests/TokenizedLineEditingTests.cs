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


        private TokenizedLine SimpleLine()
        {
            var line = new TokenizedLine();
            line.Add("this", false);
            line.Add(" ", true);
            line.Add("is", false);
            line.Add(" ", true);
            line.Add("testing", false);
            return line;
        }
    }
}