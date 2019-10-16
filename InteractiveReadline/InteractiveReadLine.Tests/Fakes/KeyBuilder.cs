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

        public KeyBuilder Add(ConsoleKey key, bool shift, bool alt, bool control)
        {
            _keys.Add(new ConsoleKeyInfo(char.MinValue, key, shift, alt, control));
            return this;
        }

        public KeyBuilder Enter()
        {
            return this.Add(ConsoleKey.Enter, false, false, false);
        }
        public KeyBuilder Escape()
        {
            return this.Add(ConsoleKey.Escape, false, false, false);
        }


        public ConsoleKeyInfo[] Keys => _keys.ToArray();

        public static KeyBuilder Create() => new KeyBuilder();
    }
}