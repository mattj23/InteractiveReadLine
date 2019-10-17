using System.Linq;
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

        [Fact]
        public void Tokenizer_SplitOnUnequalSpaces_WithCursor_WorksCorrectly()
        {
            var test = new Tokenize("this  is    a test", 15);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);
            
            Assert.Equal("this", tokens[0].Text);
            Assert.Equal("is", tokens[1].Text);
            Assert.Equal("a", tokens[2].Text);
            Assert.Equal("test", tokens[3].Text);
           
            Assert.Equal(1, tokens[3].CursorPos);
        }

        [Fact]
        public void Tokenizer_PreviousAndNext_Separators_Correct()
        {
            var test = new Tokenize("this  is    a test", 15);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);
            
            Assert.Equal("", tokens[0].PrevSeparator.Text);
            Assert.Equal("  ", tokens[0].NextSeparator.Text);
            
            Assert.Equal("  ", tokens[1].PrevSeparator.Text);
            Assert.Equal("    ", tokens[1].NextSeparator.Text);

            Assert.Equal("    ", tokens[2].PrevSeparator.Text);
            Assert.Equal(" ", tokens[2].NextSeparator.Text);

            Assert.Equal(" ", tokens[3].PrevSeparator.Text);
            Assert.Equal("", tokens[3].NextSeparator.Text);
        }

        [Fact]
        public void Tokens_Linked_Correctly_Backwards()
        {
            var test = new Tokenize("this  is    a test", 15);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            var last = tokens.Last();

            Assert.Equal("this", last.Previous.Previous.Previous.Text);
            Assert.Equal("is", last.Previous.Previous.Text);
            Assert.Equal("a", last.Previous.Text);
            Assert.Equal("test", last.Text);
        }

        [Fact]
        public void Tokens_Linked_Correctly_Forward()
        {
            var test = new Tokenize("this  is    a test", 15);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            var first = tokens.First();

            Assert.Equal("this", first.Text);
            Assert.Equal("is", first.Next.Text);
            Assert.Equal("a", first.Next.Next.Text);
            Assert.Equal("test", first.Next.Next.Next.Text);
        }

        [Fact]
        public void Tokenizer_CursorAtEndWithEmptySep_CreatesNoEmptyToken()
        {
            var test = new Tokenize("this is a test", 14);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal(4, tokens.Count);
            Assert.Equal("test", tokens.Last().Text);
        }

        [Fact]
        public void Tokenizer_CursorAtEndWithNonEmptySep_CreatesEmptyToken()
        {
            var test = new Tokenize("this is a test ", 15);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal(5, tokens.Count);
            Assert.Equal("", tokens.Last().Text);
        }

        [Fact]
        public void Tokenizer_CursorOnSecondToken_Start_Correct()
        {
            var test = new Tokenize("this is a test", 5);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal(0, tokens[1].CursorPos);
        }

        [Fact]
        public void Tokenizer_CursorOnSecondToken_End_Correctly()
        {
            var test = new Tokenize("this is a test", 6);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal(1, tokens[1].CursorPos);
        }

        [Fact]
        public void Tokenizer_InitialSeparator_WithCursor_WorksCorrectly()
        {
            var test = new Tokenize("   this is a test ", 13);
            var tokens = CommonTokenizers.SplitOnSpaces().Invoke(test);

            Assert.Equal("   ", tokens.First().PrevSeparator.Text);
            Assert.Equal(0, tokens.Last().CursorPos);
        }
    }
}