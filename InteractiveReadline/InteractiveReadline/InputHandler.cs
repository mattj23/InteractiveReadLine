using System;
using System.Runtime.InteropServices;
using System.Text;
using InteractiveReadLine.Components;
using InteractiveReadLine.KeyBehaviors;

namespace InteractiveReadLine
{
    /// <summary>
    /// This class handles getting a single line of input from the underlying ReadLine provider. It reads keys from the
    /// provider, determines what the current line of text being edited should be and where the cursor should be positioned.
    /// It pushes out the text to the provider, which also serves as the view.
    /// </summary>
    public class InputHandler : IKeyBehaviorTarget
    {
        private readonly IReadLine _provider;
        private readonly StringBuilder _content;
        private readonly ReadLineConfig _config;
        private int _cursorPos;

        public InputHandler(IReadLine provider, ReadLineConfig config=null)
        {
            _config = config ?? ReadLineConfig.Empty();
            _provider = provider;
            _content = new StringBuilder();
            _cursorPos = 0;
        }

        public StringBuilder TextBuffer => _content;

        public int CursorPosition 
        { 
            get => _cursorPos;
            set => _cursorPos = value;
        }

        public string ReadLine()
        {
            while (true)
            {
                var keyInfo = _provider.ReadKey();

                if (_config?.IsTesting == true)
                {
                }

                // See if there's a specific behavior which should be mapped to this key,
                // and if so, run it instead of checking the insert/enter behaviors
                var behavior = this.GetKeyAction(keyInfo);
                if (behavior != null)
                {
                    behavior.Invoke(this);
                }
                else
                {
                    if (keyInfo.Key == ConsoleKey.Enter)
                        break;

                    if (keyInfo.Key != ConsoleKey.Backspace)
                    {
                        _content.Insert(_cursorPos, keyInfo.KeyChar);
                        _cursorPos++;
                    }
                }

                _provider.SetInputText(_content.ToString(), _cursorPos);
            }

            return _content.ToString();
        }

        /// <summary>
        /// Check a ConsoleKeyInfo to see if the configuration object has a behavior registered
        /// for that key. The character is checked first, and if that fails, the ConsoleKey and the modifier keys
        /// are checked. If that fails, null is returned
        /// </summary>
        private Action<IKeyBehaviorTarget> GetKeyAction(ConsoleKeyInfo info)
        {
            var charKey = new KeyId(info.KeyChar);
            if (_config.KeyBehaviors.ContainsKey(charKey))
                return _config.KeyBehaviors[charKey];

            var key = new KeyId(info.Key, (info.Modifiers & ConsoleModifiers.Control) != 0,
                (info.Modifiers & ConsoleModifiers.Alt) != 0, (info.Modifiers & ConsoleModifiers.Shift) != 0);
            if (_config.KeyBehaviors.ContainsKey(key))
                return _config.KeyBehaviors[key];

            return null;
        }

    }
}