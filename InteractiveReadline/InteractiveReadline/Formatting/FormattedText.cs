using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices.ComTypes;

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

        public int Length => Text.Length;

        public FormattedChar this[int index] => new FormattedChar(this.Text[index], this.Foreground[index], this.Background[index]);
        
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

        public static FormattedText operator +(FormattedText lhs, FormattedText rhs)
        {
            var product = new FormattedText(lhs.Text + rhs.Text);
            for (int i = 0; i < lhs.Text.Length; i++)
            {
                product.Foreground[i] = lhs.Foreground[i];
                product.Background[i] = lhs.Background[i];
            }

            for (int i = 0; i < rhs.Text.Length; i++)
            {
                product.Foreground[i + lhs.Text.Length] = rhs.Foreground[i];
                product.Background[i + lhs.Text.Length] = rhs.Background[i];
            }

            return product;
        }

        public struct FormattedChar : IEquatable<FormattedChar>
        {

            public FormattedChar(char c, ConsoleColor? foreground, ConsoleColor? background)
            {
                Char = c;
                Foreground = foreground;
                Background = background;
            }

            public char Char { get; }
            public ConsoleColor? Foreground { get; }
            public ConsoleColor? Background { get; }

            public bool Equals(FormattedChar other)
            {
                return Char == other.Char && Foreground == other.Foreground && Background == other.Background;
            }

            public override bool Equals(object obj)
            {
                return obj is FormattedChar other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Char.GetHashCode();
                    hashCode = (hashCode * 397) ^ Foreground.GetHashCode();
                    hashCode = (hashCode * 397) ^ Background.GetHashCode();
                    return hashCode;
                }
            }
        }

    }
}