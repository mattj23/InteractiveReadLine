using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class TokenizeSegmentationTest
    {
        [Fact]
        public void Tokenizer_SplitOnSpaces_WorksCorrectly()
        {
            var test = new Tokenize("this is a test", 0);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal("this", tokens[0].Text);
            Assert.Equal("is", tokens[1].Text);
            Assert.Equal("a", tokens[2].Text);
            Assert.Equal("test", tokens[3].Text);
        }
    }
}