using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class TokenizeCursorTests
    {
        [Fact]
        public void CursorTestFixture_IdentifiesCursorPosition_Start()
        {
            var tokenize = FromTestText(
                "this is a test",
                "^");

            Assert.Equal(0, tokenize.Cursor);
        }

        [Fact]
        public void CursorTestFixture_IdentifiesCursorPosition_End()
        {
            var tokenize = FromTestText(
                "this is a test",
                "              ^");

            Assert.Equal(14, tokenize.Cursor);
        }

        [Fact]
        public void CursorTestFixture_IdentifiesCursorPosition_Middle()
        {
            var tokenize = FromTestText(
                "this is a test",
                "      ^");

            Assert.Equal(6, tokenize.Cursor);
        }

        [Fact]
        public void CursorInSeparatorText_IdentifiesCorrectly()
        {
            var tokenize = FromTestText(
                "this    is a test",
                "      ^          ");
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);
            Assert.Equal(1, tokens.CursorTokenIndex);
            Assert.Equal(2, tokens.CursorToken.Cursor);

        }

        [Theory]
        //           43210
        //          "    "
        [InlineData("^   ", 0)]
        [InlineData("  ^ ", 2)]
        [InlineData("   ^", 3)]
        public void CursorInSeparatorText_PrefixOnly_IdentifiesCorrectly(string cursorText, int position)
        {
            var tokenize = FromTestText("    ", cursorText);
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);

            Assert.Equal(0, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.Cursor);
        }

        [Fact]
        public void CursorInSeparatorText_MidLine_IdentifiesCorrectly()
        {
            var tokenize = FromTestText(
                "this    is   a test",
                "           ^       ");
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);
            Assert.Equal(3, tokens.CursorTokenIndex);
            Assert.Equal(1, tokens.CursorToken.Cursor);
        }

        [Theory]
        //             0  2  4   6  8  10    12
        //           0123 01 0 0123 01 012 012345
        //          "this is a test of the cursor"
        [InlineData("^                            ", 0, 0)]
        [InlineData(" ^                           ", 0, 1)]
        [InlineData("    ^                        ", 0, 4)]
        [InlineData("          ^                  ", 6, 0)]
        [InlineData("           ^                 ", 6, 1)]
        [InlineData("              ^              ", 6, 4)]
        [InlineData("                      ^      ", 12, 0)]
        [InlineData("                           ^ ", 12, 5)]
        [InlineData("                            ^", 12, 6)]
        public void TokenizedCursor_SimpleCase_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText("this is a test of the cursor", cursorText);
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.Cursor);
        }

        [Theory]
        //            1    3  5   7  9  11    13
        //            0123 01 0 0123 01 012 012345
        //          " this is a test of the cursor"
        [InlineData(" ^                            ", 1, 0)]
        [InlineData("  ^                           ", 1, 1)]
        [InlineData("     ^                        ", 1, 4)]
        [InlineData("           ^                  ", 7, 0)]
        [InlineData("            ^                 ", 7, 1)]
        [InlineData("               ^              ", 7, 4)]
        [InlineData("                       ^      ", 13, 0)]
        [InlineData("                            ^ ", 13, 5)]
        [InlineData("                             ^", 13, 6)]
        public void TokenizedCursor_WithLeadingSpace_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText(" this is a test of the cursor", cursorText);
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.Cursor);
        }

        [Theory]
        //             0  2  4   6  8  10    12
        //           0123 01 0 0123 01 012 012345
        //          "this is a test of the cursor "
        [InlineData("                      ^        ", 12, 0)]
        [InlineData("                           ^   ", 12, 5)]
        [InlineData("                            ^  ", 12, 6)]
        [InlineData("                              ^", 14, 0)]
        public void TokenizedCursor_TrailingSpace_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText("this is a test of the cursor ", cursorText);
            var tokens = CommonLexers.SplitOnWhitespace(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.Cursor);
        }

        public LineState FromTestText(string text, string curs)
        {
            int cpos = curs.IndexOf('^');
            return new LineState(text, cpos);
        }
    }
}