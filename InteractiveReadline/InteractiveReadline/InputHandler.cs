using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

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
        private int _autoCompleteIndex;
        private TokenizedLine _autoCompleteTokens;
        private bool _autoCompleteCalled = false;
        private string[] _autoCompleteSuggestions;

        public InputHandler(IReadLine provider, ReadLineConfig config=null)
        {
            _config = config ?? ReadLineConfig.Empty();
            _provider = provider;
            _content = new StringBuilder();
            _cursorPos = 0;

            _autoCompleteIndex = Int32.MinValue;
            _autoCompleteSuggestions = null;
        }

        public StringBuilder TextBuffer => _content;

        public int CursorPosition 
        { 
            get => _cursorPos;
            set => _cursorPos = value;
        }

        public void AutoCompleteNext()
        {
            if (_autoCompleteIndex >= 0)
            {
                // Next index
                _autoCompleteIndex++;
                if (_autoCompleteIndex >= _autoCompleteSuggestions.Length)
                    _autoCompleteIndex = 0;

                this.SetAutoCompleteText();
            }
            else
                this.StartAutoComplete();
        }

        public void AutoCompletePrevious()
        {
            if (_autoCompleteIndex >= 0)
            {
                // Previous index
                _autoCompleteIndex--;
                if (_autoCompleteIndex < 0)
                    _autoCompleteIndex = _autoCompleteSuggestions.Length - 1;

                this.SetAutoCompleteText();
            }
            else 
                this.StartAutoComplete();

        }

        public void WriteMessage(FormattedText text)
        {
            _provider.WriteMessage(text);
        }

        public LineState LineState => new LineState(_content.ToString(), _cursorPos);

        public TokenizedLine GetTextTokens()
        {
            return _config.Lexer?.Invoke(this.LineState);
        }

        public string ReadLine()
        {
            this.UpdateDisplay();

            while (true)
            {
                var keyInfo = _provider.ReadKey();

                _autoCompleteCalled = false;
                var textContents = _content.ToString();
                var cursor = _cursorPos;
                
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

                // If the text contents or the cursor have changed at all, and we weren't currently
                // doing autocomplete, we need to invalidate the autocompletion information
                if ((textContents != _content.ToString() || cursor != _cursorPos) && !_autoCompleteCalled)
                    this.InvalidateAutoComplete();

                this.UpdateDisplay();
            }

            return _content.ToString();
        }

        private void UpdateDisplay()
        {
            // Finally, if we have an available formatter, we can get a display format from here
            var display = new LineDisplayState(string.Empty, _content.ToString(), string.Empty, _cursorPos);

            if (_config.FormatterFromLine != null)
                display = _config.FormatterFromLine.Invoke(LineState);
            else if (_config.FormatterFromTokens != null && _config.Lexer != null)
                display = _config.FormatterFromTokens(GetTextTokens());

            _provider.SetDisplay(display);
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

        private void StartAutoComplete()
        {
            if (!_config.CanAutoComplete)
                return;

            _autoCompleteTokens = _config.Lexer(new LineState(_content.ToString(), _cursorPos));
            if (_autoCompleteTokens.CursorToken == null)
                return;

            _autoCompleteSuggestions = _config.AutoCompletion(_autoCompleteTokens) ?? Array.Empty<string>();

            if (_autoCompleteTokens.Text != _content.ToString())
            {
                _content.Clear();
                _content.Append(_autoCompleteTokens.Text);
                CursorPosition = _autoCompleteTokens.Cursor;
            }

            if (_autoCompleteSuggestions.Any())
            {
                _autoCompleteIndex = 0;
                SetAutoCompleteText();
            }

        }

        private void InvalidateAutoComplete()
        {
            _autoCompleteIndex = Int32.MinValue;
            _autoCompleteTokens = null;
            _autoCompleteSuggestions = null;
        }

        private void SetAutoCompleteText()
        {
            if (!_config.CanAutoComplete || _autoCompleteTokens == null || _autoCompleteIndex < 0)
                return;

            _autoCompleteCalled = true;
            _autoCompleteTokens.CursorToken.Text = _autoCompleteSuggestions[_autoCompleteIndex];
            _autoCompleteTokens.CursorToken.Cursor = _autoCompleteTokens.CursorToken.Text.Length;

            _content.Clear();
            _content.Append(_autoCompleteTokens.Text);
            CursorPosition = _autoCompleteTokens.Cursor;

        }
    }
}