using System;
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
    }
}