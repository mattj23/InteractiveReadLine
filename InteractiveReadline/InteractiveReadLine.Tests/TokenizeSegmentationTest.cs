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
            var test = new LineState("this is a test", 0);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();

            Assert.Equal("this", tokens[0].Text);
            Assert.Equal("is", tokens[1].Text);
            Assert.Equal("a", tokens[2].Text);
            Assert.Equal("test", tokens[3].Text);
        }

        [Fact]
        public void Tokenizer_SplitOnUnequalSpaces_WithCursor_WorksCorrectly()
        {
            var test = new LineState("this  is    a test", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();
            
            Assert.Equal("this", tokens[0].Text);
            Assert.Equal("is", tokens[1].Text);
            Assert.Equal("a", tokens[2].Text);
            Assert.Equal("test", tokens[3].Text);
           
            Assert.Equal(1, tokens[3].Cursor);
        }

        [Fact]
        public void Tokenizer_PreviousAndNext_Separators_Correct()
        {
            var test = new LineState("this  is    a test", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => x.IsHidden).ToArray();

            Assert.Equal("  ", tokens[0].Text);
            Assert.Equal("    ", tokens[1].Text);
            Assert.Equal(" ", tokens[2].Text);
        }

        [Fact]
        public void Tokenizer_SplitOnUnequalSpaces_Linking_WorksCorrectly()
        {
            var test = new LineState("this  is    a test", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();
            
            Assert.Equal("this", tokens[0].Text);
            Assert.Equal("  ", tokens[0].Next.Text);

            Assert.Equal("  ", tokens[1].Previous.Text);
            Assert.Equal("is", tokens[1].Text);
            Assert.Equal("    ", tokens[1].Next.Text);

            Assert.Equal("    ", tokens[2].Previous.Text);
            Assert.Equal("a", tokens[2].Text);
            Assert.Equal(" ", tokens[2].Next.Text);
        
            Assert.Equal(" ", tokens[3].Previous.Text);
        }
        
        [Fact]
        public void Tokens_Linked_Correctly_Backwards()
        {
            var test = new LineState("this  is    a test", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();

            var last = tokens.Last();

            Assert.Equal("this", last.PreviousNotHidden.PreviousNotHidden.PreviousNotHidden.Text);
            Assert.Equal("is", last.PreviousNotHidden.PreviousNotHidden.Text);
            Assert.Equal("a", last.PreviousNotHidden.Text);
            Assert.Equal("test", last.Text);
        }

        [Fact]
        public void Tokens_Linked_Correctly_Forward()
        {
            var test = new LineState("this  is    a test", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();

            var first = tokens.First();

            Assert.Equal("this", first.Text);
            Assert.Equal("is", first.NextNotHidden.Text);
            Assert.Equal("a", first.NextNotHidden.NextNotHidden.Text);
            Assert.Equal("test", first.NextNotHidden.NextNotHidden.NextNotHidden.Text);
        }

        [Fact]
        public void Tokenizer_CursorAtEndWithEmptySep_CreatesNoEmptyToken()
        {
            var test = new LineState("this is a test", 14);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);
            var tokens = tokenized.Where(x => !x.IsHidden).ToArray();

            Assert.Equal(4, tokens.Length);
            Assert.Equal("test", tokenized.Last.Text);
        }

        [Fact]
        public void Tokenizer_CursorAtEndWithNonEmptySep_CreatesEmptyToken()
        {
            var test = new LineState("this is a test ", 15);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);

            Assert.Equal(5, tokenized.Count(x => !x.IsHidden));
            Assert.Equal("", tokenized.Last.Text);
        }

        [Fact]
        public void Tokenizer_CursorOnSecondToken_Start_Correct()
        {
            var test = new LineState("this is a test", 5);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);

            Assert.Equal(0, tokenized[2].Cursor);
        }

        [Fact]
        public void Tokenizer_CursorOnSecondToken_End_Correctly()
        {
            var test = new LineState("this is a test", 6);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);

            Assert.Equal(1, tokenized[2].Cursor);
        }

        [Fact]
        public void Tokenizer_InitialSeparator_WithCursor_WorksCorrectly()
        {
            var test = new LineState("   this is a test ", 13);
            var tokenized = CommonTokenizers.SplitOnWhitespace(test);

            Assert.Equal("   ", tokenized.First.Text);
            Assert.Equal(0, tokenized.Last(x => !x.IsHidden).Cursor);
        }
    }
}