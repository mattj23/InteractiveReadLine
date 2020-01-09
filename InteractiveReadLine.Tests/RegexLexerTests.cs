using System;
using System.Linq;
using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class RegexLexerTests
    {
        [Fact]
        public void EmptyRegex_ToLexer_ThrowsArgException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var lexer = CommonLexers.Regex.ToLexer();
            });
        }

        [Fact]
        public void AddTokenType_WithoutStartChar_ThrowsArgException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var lexer = CommonLexers.Regex.AddTokenType(@"\d+");
            });
        }

        [Fact]
        public void SingleRegex_ToLexer_Functions()
        {
            // Create a lexer that only considers digits valid
            var lexer = CommonLexers.Regex.AddTokenType(@"^\d+", 1).ToLexer();

            var test = new LineState("this is 2031 a test", 0);
            var tokenized = lexer(test);

            Assert.Equal("2031", tokenized.FirstNonHidden.Text);
            Assert.Equal(1, tokenized.FirstNonHidden.TypeCode);
        }

        [Fact]
        public void TwoRegex_ToLexer_Functions()
        {
            // Since the integer literal will match the beginning of a floating point value,
            // it is important that they are put so that the more specific regex comes before
            // the more general one
            var lexer = CommonLexers.Regex
                .AddTokenType(@"^-?[0-9]*\.[0-9]*", 1)  // floating point literal
                .AddTokenType(@"^-?\d+", 2)   // integer literal
                .ToLexer();

            var test = new LineState("this 58.34 is 2031 a test", 0);
            var tokenized = lexer(test);

            var nonHidden = tokenized.Where(x => !x.IsHidden).ToArray();

            Assert.Equal(2, nonHidden.Length);
            Assert.Equal("58.34", nonHidden[0].Text);
            Assert.Equal(1, nonHidden[0].TypeCode);

            Assert.Equal("2031", nonHidden[1].Text);
            Assert.Equal(2, nonHidden[1].TypeCode);
        }

        [Fact]
        public void TwoRegexWrongOrder_ToLexer_FunctionsWrong()
        {
            var lexer = CommonLexers.Regex
                .AddTokenType(@"^-?\d+", 2)   // integer literal
                .AddTokenType(@"^-?[0-9]*\.[0-9]*", 1)  // floating point literal
                .ToLexer();

            var test = new LineState("this 58.34 is 2031 a test", 0);
            var tokenized = lexer(test);

            var nonHidden = tokenized.Where(x => !x.IsHidden).ToArray();

            Assert.Equal(3, nonHidden.Length);
            Assert.Equal("58", nonHidden[0].Text);
            Assert.Equal(2, nonHidden[0].TypeCode);

            Assert.Equal(".34", nonHidden[1].Text);
            Assert.Equal(1, nonHidden[1].TypeCode);

            Assert.Equal("2031", nonHidden[2].Text);
            Assert.Equal(2, nonHidden[2].TypeCode);
        }
    }
}