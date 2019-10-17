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
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);
            Assert.Equal(0, tokens.CursorTokenIndex);
            Assert.Equal(6, tokens.CursorToken.CursorPos);

        }

        [Theory]
        //           432101234
        //          "    this "
        [InlineData("^        ", -4)]
        [InlineData("  ^      ", -2)]
        [InlineData("   ^     ", -1)]
        public void CursorInSeparatorText_Prefix_IdentifiesCorrectly(string cursorText, int position)
        {
            var tokenize = FromTestText("    this ", cursorText);
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);

            Assert.Equal(0, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.CursorPos);
        }

        [Fact]
        public void CursorInSeparatorText_MidLine_IdentifiesCorrectly()
        {
            var tokenize = FromTestText(
                "this    is   a test",
                "           ^       ");
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);
            Assert.Equal(1, tokens.CursorTokenIndex);
            Assert.Equal(3, tokens.CursorToken.CursorPos);
        }

        [Theory]
        //             0  1  2   3  4  5    6
        //           0123 01 0 0123 01 012 012345
        //          "this is a test of the cursor"
        [InlineData("^                            ", 0, 0)]
        [InlineData(" ^                           ", 0, 1)]
        [InlineData("    ^                        ", 0, 4)]
        [InlineData("          ^                  ", 3, 0)]
        [InlineData("           ^                 ", 3, 1)]
        [InlineData("              ^              ", 3, 4)]
        [InlineData("                      ^      ", 6, 0)]
        [InlineData("                           ^ ", 6, 5)]
        [InlineData("                            ^", 6, 6)]
        public void TokenizedCursor_SimpleCase_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText("this is a test of the cursor", cursorText);
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.CursorPos);
        }

        [Theory]
        //             0  1  2   3  4  5    6
        //            0123 01 0 0123 01 012 012345
        //          " this is a test of the cursor"
        [InlineData(" ^                            ", 0, 0)]
        [InlineData("  ^                           ", 0, 1)]
        [InlineData("     ^                        ", 0, 4)]
        [InlineData("           ^                  ", 3, 0)]
        [InlineData("            ^                 ", 3, 1)]
        [InlineData("               ^              ", 3, 4)]
        [InlineData("                       ^      ", 6, 0)]
        [InlineData("                            ^ ", 6, 5)]
        [InlineData("                             ^", 6, 6)]
        public void TokenizedCursor_WithLeadingSpace_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText(" this is a test of the cursor", cursorText);
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.CursorPos);
        }

        [Theory]
        //             0  1  2   3  4  5    6
        //           0123 01 0 0123 01 012 012345
        //          "this is a test of the cursor "
        [InlineData("                      ^        ", 6, 0)]
        [InlineData("                           ^   ", 6, 5)]
        [InlineData("                            ^  ", 6, 6)]
        [InlineData("                              ^", 7, 0)]
        public void TokenizedCursor_TrailingSpace_IdentifiesCorrectToken(string cursorText, int token, int position)
        {
            var tokenize = FromTestText("this is a test of the cursor ", cursorText);
            var tokens = CommonTokenizers.SplitOnSpaces(tokenize);

            Assert.Equal(token, tokens.CursorTokenIndex);
            Assert.Equal(position, tokens.CursorToken.CursorPos);
        }

        public Tokenize FromTestText(string text, string curs)
        {
            int cpos = curs.IndexOf('^');
            return new Tokenize(text, cpos);
        }
    }
}