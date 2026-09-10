using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InteractiveReadLine.Tests.Fakes
{
    public class KeyBuilder
    {
        private readonly List<ConsoleKeyInfo> _keys;

        private KeyBuilder()
        {
            _keys = new List<ConsoleKeyInfo>();
        }

        public KeyBuilder Add(string text)
        {
            foreach (var c in text)
            {
                var cstr = c.ToString();
                var code = (int) cstr.ToUpper()[0];
                ConsoleKey key = ConsoleKey.Oem1; // Something benign 
                try
                {
                    key = (ConsoleKey) code;
                }
                catch
                {
                    ;
                }
                bool shift = cstr != cstr.ToLower();
                var keyInfo = new ConsoleKeyInfo(c, key, shift, false, false);
                _keys.Add(keyInfo);
            }

            return this;
        }

        public KeyBuilder Add(ConsoleKey key, bool shift, bool alt, bool control, int count=1)
        {
            for (int i = 0; i < count; i++)
            {
                _keys.Add(new ConsoleKeyInfo(char.MinValue, key, shift, alt, control));
            }

            return this;
        }

        public KeyBuilder Enter(int count=1)
        {
            return this.Add(ConsoleKey.Enter, false, false, false, count);
        }

        public KeyBuilder LeftArrow(int count=1)
        {
            return this.Add(ConsoleKey.LeftArrow, false, false, false, count);
        }

        public KeyBuilder RightArrow(int count=1)
        {
            return this.Add(ConsoleKey.RightArrow, false, false, false, count);
        }

        public KeyBuilder DownArrow(int count=1)
        {
            return this.Add(ConsoleKey.DownArrow, false, false, false, count);
        }

        public KeyBuilder UpArrow(int count=1)
        {
            return this.Add(ConsoleKey.UpArrow, false, false, false, count);
        }

        public KeyBuilder Escape(int count=1)
        {
            return this.Add(ConsoleKey.Escape, false, false, false, count);
        }

        /// <summary>
        /// Adds a Ctrl+C keypress as the console delivers it when TreatControlCAsInput is set. The keypress
        /// carries the end-of-text control character instead of a 'c'.
        /// </summary>
        public KeyBuilder CtrlC(int count=1)
        {
            return this.AddControlChar(ConsoleKey.C, '\u0003', count);
        }

        /// <summary>
        /// Adds a Ctrl+D keypress, which carries the end-of-transmission control character.
        /// </summary>
        public KeyBuilder CtrlD(int count=1)
        {
            return this.AddControlChar(ConsoleKey.D, '\u0004', count);
        }

        /// <summary>
        /// Adds a Ctrl+letter keypress as the console delivers it. The keypress carries the matching control
        /// character instead of the letter. Use this method only with letter keys A through Z.
        /// </summary>
        public KeyBuilder Ctrl(ConsoleKey key, int count=1)
        {
            var character = (char) (key - ConsoleKey.A + 1);
            return this.AddControlChar(key, character, count);
        }

        private KeyBuilder AddControlChar(ConsoleKey key, char character, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _keys.Add(new ConsoleKeyInfo(character, key, false, false, true));
            }

            return this;
        }


        public ConsoleKeyInfo[] Keys => _keys.ToArray();

        public static KeyBuilder Create() => new KeyBuilder();
    }
}
