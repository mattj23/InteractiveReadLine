using System;
using System.Linq;
using InteractiveReadLine.Formatting;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class FormattedTextTests
    {
        [Fact]
        public void FormattedTextConstructor_InstantiatesText_Correctly()
        {
            string t = "this is some text";
            var test = new FormattedText(t);

            Assert.Equal(t, test.Text);
        }

        [Fact]
        public void FormattedTextConstructor_InstantiatesWithNullColors()
        {
            string t = "this is some text";
            var test = new FormattedText(t);

            Assert.All(test.Foreground, color => Assert.Null(color));
            Assert.All(test.Background, color => Assert.Null(color));
        }

        [Fact]
        public void FormattedText_SetForegroundColors_Works()
        {
            string t = "this is some text";
            var test = new FormattedText(t);
            test.SetForeground(ConsoleColor.Blue);

            Assert.All(test.Foreground, color => Assert.Equal(ConsoleColor.Blue, color));
        }

        [Fact]
        public void FormattedText_SetBackgroundColors_Works()
        {
            string t = "this is some text";
            var test = new FormattedText(t);
            test.SetBackground(ConsoleColor.Blue);

            Assert.All(test.Background, color => Assert.Equal(ConsoleColor.Blue, color));
        }

        [Fact]
        public void FormattedText_ImplictToString_Works()
        {
            string t = "this is some text";
            var converter = new Func<FormattedText, FormattedText>(s => s);
            var test = converter(t);

            Assert.Equal(typeof(FormattedText), test.GetType());
            Assert.Equal(t, test.Text);
            Assert.All(test.Foreground, color => Assert.Null(color));
            Assert.All(test.Background, color => Assert.Null(color));
        }

        [Fact]
        public void FormattedText_Concat_HandlesColorsCorrectly()
        {
            var t1 = new FormattedText("first");
            var t2 = new FormattedText("second");
            t1.SetForeground(ConsoleColor.Red);
            t1.SetBackground(ConsoleColor.Black);
            t2.SetForeground(ConsoleColor.Blue);
            t2.SetBackground(ConsoleColor.White);

            var test = t1 + t2;

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(ConsoleColor.Red, test.Foreground[i]);
                Assert.Equal(ConsoleColor.Black, test.Background[i]);
            }

            for (int i = 5; i < 10; i++)
            {
                Assert.Equal(ConsoleColor.Blue, test.Foreground[i]);
                Assert.Equal(ConsoleColor.White, test.Background[i]);
            }
        }

        [Fact]
        public void Equality_FalseOnNullOther()
        {
            var t = new FormattedText("test");
            
            Assert.False(t.Equals(null));
        }

        [Fact]
        public void Equality_FalseOnMismatchedText()
        {
            var t0 = new FormattedText("test");
            var t1 = new FormattedText("t");
            
            Assert.NotEqual(t0, t1);
        }
        
        [Fact]
        public void Equality_FalseOnMismatchedForeground()
        {
            var t0 = new FormattedText("te", ConsoleColor.Black, ConsoleColor.White) + 
                new FormattedText("s", null, ConsoleColor.White) + 
                new FormattedText("t", ConsoleColor.Black, ConsoleColor.White);
            var t1 = new FormattedText("test", ConsoleColor.Black, ConsoleColor.White);
            
            Assert.NotEqual(t0, t1);
        }
        
        [Fact]
        public void Equality_FalseOnMismatchedBackground()
        {
            var t0 = new FormattedText("te", ConsoleColor.Black, ConsoleColor.White) + 
                new FormattedText("s", ConsoleColor.Black, null) + 
                new FormattedText("t", ConsoleColor.Black, ConsoleColor.White);
            var t1 = new FormattedText("test", ConsoleColor.Black, ConsoleColor.White);
            
            Assert.NotEqual(t0, t1);
        }
        
        [Fact]
        public void Equality_TrueOnMatch()
        {
            var t0 = new FormattedText("te", ConsoleColor.Black, ConsoleColor.White) + 
                new FormattedText("s", ConsoleColor.Black, ConsoleColor.White) + 
                new FormattedText("t", ConsoleColor.Black, ConsoleColor.White);
            var t1 = new FormattedText("test", ConsoleColor.Black, ConsoleColor.White);
            
            Assert.Equal(t0, t1);
        }
        
        [Fact]
        public void SplitByFormatting_HandlesEmptyString()
        {
            var t = new FormattedText(string.Empty);

            var split = t.SplitByFormatting();
            
            Assert.Empty(split);
        }
        
        [Fact]
        public void SplitByFormatting_HandlesSingleCharacterString()
        {
            var t = new FormattedText("t");

            var split = t.SplitByFormatting();
            
            Assert.Single(split);
            Assert.Equal("t", split.First().Text);
        }
        
        [Theory]
        [InlineData(null, null)]
        [InlineData(ConsoleColor.Red, null)]
        [InlineData(null, ConsoleColor.Black)]
        [InlineData(ConsoleColor.Blue, ConsoleColor.Black)]
        public void SplitByFormatting_HandlesSingleGroup(ConsoleColor? fore, ConsoleColor? back)
        {
            var t = new FormattedText("this is some test text", fore, back);

            var split = t.SplitByFormatting();

            Assert.Single(split);
            Assert.Equal(fore, split.First()[0].Foreground);
            Assert.Equal(back, split.First()[0].Background);
            Assert.Equal(t.Text, split.First().Text);
        }

        [Fact]
        public void SplitByFormatting_SplitsCorrectly()
        {
            var t0 = new FormattedText("this ", ConsoleColor.Black, ConsoleColor.White);
            var t1 = new FormattedText("is ");
            var t2 = new FormattedText("some ", ConsoleColor.Black, ConsoleColor.White);
            var t3 = new FormattedText("test ", ConsoleColor.Red, ConsoleColor.White);
            var t4 = new FormattedText("text", ConsoleColor.Red, null);
            var t = t0 + t1 + t2 + t3 + t4;

            var split = t.SplitByFormatting();
            
            Assert.Equal(5, split.Length);
            Assert.Equal(t0, split[0]); 
            Assert.Equal(t1, split[1]); 
            Assert.Equal(t2, split[2]); 
            Assert.Equal(t3, split[3]); 
            Assert.Equal(t4, split[4]); 
        }
    }
}