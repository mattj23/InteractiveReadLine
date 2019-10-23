using System;

namespace InteractiveReadLine.Formatting
{
    public class FormattedText
    {
        public FormattedText(string text)
        {
            this.Text = text;
            this.Foreground = new ConsoleColor?[this.Text.Length];
            this.Background = new ConsoleColor?[this.Text.Length];

            for (int i = 0; i < this.Text.Length; i++)
            {
                this.Foreground[i] = null;
                this.Background[i] = null;
            }
        }

        public string Text { get; }

        public ConsoleColor?[] Foreground { get; }

        public ConsoleColor?[] Background { get; }

        public void SetForeground(ConsoleColor? color)
        {
            for (int i = 0; i < this.Text.Length; i++)
            {
                Foreground[i] = color;
            }
        }

        public void SetBackground(ConsoleColor? color)
        {
            for (int i = 0; i < this.Text.Length; i++)
            {
                Background[i] = color;
            }
        }

        public static implicit operator FormattedText(string s) => new FormattedText(s);
    }
}