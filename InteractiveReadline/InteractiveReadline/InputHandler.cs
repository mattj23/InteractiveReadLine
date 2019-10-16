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
    public class InputHandler : IDisposable
    {
        private readonly IReadLine _provider;
        private readonly StringBuilder _content;

        public InputHandler(IReadLine provider)
        {
            _provider = provider;
            _content = new StringBuilder();
        }

        public string ReadLine()
        {
            while (true)
            {
                var keyInfo = _provider.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                _content.Append(keyInfo.KeyChar);
                _provider.SetInputText(_content.ToString(), _content.Length);
            }

            return _content.ToString();

        }

        public void Dispose()
        {
            _provider?.Dispose();
        }
    }
}