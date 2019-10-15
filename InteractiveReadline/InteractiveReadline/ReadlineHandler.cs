using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InteractiveReadLine
{
    public class ReadLineHandler
    {
        private readonly IReadLineProvider _provider;
        private readonly StringBuilder _content;

        public ReadLineHandler(IReadLineProvider provider)
        {
            _provider = provider;
            _content = new StringBuilder();
        }

        public string ReadLine(string prompt)
        {

            while (true)
            {
                var keyInfo = _provider.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                _content.Append(keyInfo.KeyChar);
                _provider.SetText(prompt + _content.ToString());
            }

            _provider.Finish();
            return _content.ToString();

        }
    }
}