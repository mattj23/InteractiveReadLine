using System;
using System.Collections.Generic;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// An identity for a key, is equatable so that it can be used in a dictionary
    /// </summary>
    public struct KeyId : IEquatable<KeyId>
    {
        /// <summary>
        /// Creates an identity that matches a character, regardless of the keys that produced it.
        /// </summary>
        /// <param name="c">The character to match.</param>
        public KeyId(char c)
        {
            this.Char = c;
            this.Key = null;
            this.HasAlt = false;
            this.HasCtrl = false;
            this.HasShift = false;
        }

        /// <summary>
        /// Creates an identity matching a key pressed together with a particular set of modifiers.
        /// </summary>
        /// <param name="key">The key to match.</param>
        /// <param name="ctrl">Whether the Control key must be held.</param>
        /// <param name="alt">Whether the Alt key must be held.</param>
        /// <param name="shift">Whether the Shift key must be held.</param>
        public KeyId(ConsoleKey key, bool ctrl, bool alt, bool shift)
        {
            this.Char = null;
            this.Key = key;
            this.HasCtrl = ctrl;
            this.HasAlt = alt;
            this.HasShift = shift;
        }

        // These properties are deliberately get-only. KeyId values serve as dictionary keys, and mutating a
        // value after using it to store a behavior would make the entry unreachable through that value.
        /// <summary>
        /// Gets the character this identity matches, or null when it matches a key rather than a character.
        /// </summary>
        public char? Char { get; }
        /// <summary>
        /// Gets the key this identity matches, or null when it matches a character rather than a key.
        /// </summary>
        public ConsoleKey? Key { get; }

        /// <summary>
        /// Gets whether the Control key must be held for this identity to match.
        /// </summary>
        public bool HasCtrl { get; }
        /// <summary>
        /// Gets whether the Alt key must be held for this identity to match.
        /// </summary>
        public bool HasAlt { get; }
        /// <summary>
        /// Gets whether the Shift key must be held for this identity to match.
        /// </summary>
        public bool HasShift { get; }

        /// <summary>
        /// Returns a readable description of the keypress, such as "Ctrl+Shift+Tab".
        /// </summary>
        public override string ToString()
        {
            var repr = new List<string>();
            if (HasCtrl)
                repr.Add("Ctrl");
            if (HasAlt)
                repr.Add("Alt");
            if (HasShift)
                repr.Add("Shift");
            if (Key != null)
                repr.Add(Key.Value.ToString());
            else 
                repr.Add("'" + Char + "'");

            return string.Join("+", repr);
        }

        /// <summary>
        /// Determines whether this identity matches the same keypress as another.
        /// </summary>
        public bool Equals(KeyId other)
        {
            return Char == other.Char && Key == other.Key && HasCtrl == other.HasCtrl && HasAlt == other.HasAlt && HasShift == other.HasShift;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is KeyId other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Char.GetHashCode();
                hashCode = (hashCode * 397) ^ Key.GetHashCode();
                hashCode = (hashCode * 397) ^ HasCtrl.GetHashCode();
                hashCode = (hashCode * 397) ^ HasAlt.GetHashCode();
                hashCode = (hashCode * 397) ^ HasShift.GetHashCode();
                return hashCode;
            }
        }
    }
}
