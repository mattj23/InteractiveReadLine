using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InteractiveReadLine
{
    /// <summary>
    /// This class handles getting a single line of input from the underlying ReadLine provider. It reads keys from the
    /// provider, determines what the current line of text being edited should be and where the cursor should be positioned.
    /// It pushes out the text to the provider, which also serves as the view.
    /// </summary>
    public class InputHandler
    {
        private readonly IReadLine _provider;
        private readonly StringBuilder _content;
        private readonly HandlerConfig _config;

        public InputHandler(IReadLine provider, HandlerConfig config=null)
        {
            _config = config;
            _provider = provider;
            _content = new StringBuilder();
        }

        public string ReadLine()
        {
            while (true)
            {
                var keyInfo = _provider.ReadKey();

                if (_config?.IsTesting == true)
                {
                }

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                _content.Append(keyInfo.KeyChar);
                _provider.SetInputText(_content.ToString(), _content.Length);
            }

            return _content.ToString();
        }

    }
}