using System;
using System.Collections.Generic;
using System.Text;

namespace InteractiveReadLine.Formatting
{
    /// <summary>
    /// Represents a formatted string, where each character has a potentially unique foreground and background
    /// color assigned to it.
    /// </summary>
    public class FormattedText : IEquatable<FormattedText>
    {
        public FormattedText(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
            this.Foreground = new ConsoleColor?[this.Text.Length];
            this.Background = new ConsoleColor?[this.Text.Length];

            for (int i = 0; i < this.Text.Length; i++)
            {
                this.Foreground[i] = foreground;
                this.Background[i] = background;
            }
        }

        public FormattedText(FormattedChar c)
            : this(c.Char.ToString(), c.Foreground, c.Background)
        {
        }

        /// <summary>
        /// Gets the characters in the text as a string without any formatting
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the length of the text 
        /// </summary>
        public int Length => Text.Length;

        /// <summary>
        /// Gets the character and its formatting at position i in the text
        /// </summary>
        /// <param name="index"></param>
        public FormattedChar this[int index] => new FormattedChar(this.Text[index], this.Foreground[index], this.Background[index]);
        
        /// <summary>
        /// Gets the foreground color array of the text 
        /// </summary>
        public ConsoleColor?[] Foreground { get; }

        /// <summary>
        /// Gets the background color array of the text
        /// </summary>
        public ConsoleColor?[] Background { get; }

        /// <summary>
        /// Sets the foreground color for every character in the text 
        /// </summary>
        /// <param name="color"></param>
        public void SetForeground(ConsoleColor? color)
        {
            for (int i = 0; i < this.Text.Length; i++)
            {
                Foreground[i] = color;
            }
        }

        /// <summary>
        /// Sets the background color for every character in the text 
        /// </summary>
        /// <param name="color"></param>
        public void SetBackground(ConsoleColor? color)
        {
            for (int i = 0; i < this.Text.Length; i++)
            {
                Background[i] = color;
            }
        }

        /// <summary>
        /// Splits the formatted text object into an array of FormattedText objects, in which each element of the array
        /// consists of text where every character has the same foreground and background colors.
        /// </summary>
        /// <returns></returns>
        public FormattedText[] SplitByFormatting()
        {
            if (this.Text == string.Empty)
            {
                return Array.Empty<FormattedText>();
            }
            
            var result = new List<FormattedText>();
            
            var buffer = new StringBuilder();
            ConsoleColor? fore = null;
            ConsoleColor? back = null;

            buffer.Append(this.Text[0]);
            fore = this.Foreground[0];
            back = this.Background[0];

            for (int i = 1; i < this.Text.Length; i++)
            {
                if (fore != this.Foreground[i] || back != this.Background[i])
                {
                    // Either the foreground or the background has changed, so we need to 
                    // finish the existing buffer and start a new one
                    result.Add(new FormattedText(buffer.ToString(), fore, back));

                    buffer.Clear();
                    fore = this.Foreground[i];
                    back = this.Background[i];
                }
                
                buffer.Append(this.Text[i]);
            }
            
            // Add the final buffer
            result.Add(new FormattedText(buffer.ToString(), fore, back));

            return result.ToArray();
        }

        public static implicit operator FormattedText(string s) => new FormattedText(s);
        
        public static implicit operator FormattedText(FormattedChar c) => new FormattedText(c);

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

        /// <summary>
        /// Determines whether this instance and another FormattedText instance contain identical characters
        /// and identical per-character foreground and background colors.
        /// </summary>
        public bool Equals(FormattedText other)
        {
            if (other == null || this.Text != other.Text)
                return false;

            for (int i = 0; i < this.Length; i++)
            {
                if (this.Foreground[i] != other.Foreground[i])
                    return false;

                if (this.Background[i] != other.Background[i])
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this.Equals(obj as FormattedText);

        /// <summary>
        /// Gets a hash code derived from the characters of the text, ignoring the colors.
        /// </summary>
        /// <remarks>
        /// The color arrays are mutable and publicly accessible, so including them would let an instance's hash
        /// code change while the instance is in a hash-based collection. Hashing only the text keeps the hash
        /// code stable for the lifetime of the instance and satisfies the contract because equal instances
        /// contain the same text. Instances that differ only by color can have the same hash code.
        /// </remarks>
        public override int GetHashCode() => this.Text.GetHashCode();
    }
}
