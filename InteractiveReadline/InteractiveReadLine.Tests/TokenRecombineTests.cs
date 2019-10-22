using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class TokenRecombineTests
    {
        [Theory]
        [InlineData("this is a test")]
        [InlineData(" this is a test")]
        [InlineData("this is a test ")]
        [InlineData("this is   a test")]
        [InlineData("  ")]
        [InlineData("test")]
        public void Tokens_RoundTrip_CombineToExpected(string text)
        {
            // Check that the cursor is reset correctly for each position in the text
            for (int i = 0; i < text.Length + 1; i++)
            {
                var tokenize = new Tokenize(text, i);
                var tokens = CommonTokenizers.SplitOnSpaces(tokenize);
                var result = tokens.Combine();

                Assert.Equal(text, result.Text);
                Assert.Equal(i, result.Cursor);
            }
        }
    }
}