using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveReadLine.Tokenizing
{
    public class Tokens : IReadOnlyList<Token>
    {
        private readonly List<Token> _tokens;

        public Tokens(IEnumerable<Token> tokens=null)
        {
            _tokens = new List<Token>(tokens ?? Enumerable.Empty<Token>());
            for (int i = 0; i < _tokens.Count - 1; i++)
            {
                _tokens[i].LinkToNext(_tokens[i+1]);
            }
        }

        public int Count => _tokens.Count;

        public Token this[int index] => _tokens[index];

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Tokens Clone() => new Tokens(_tokens.Select(t => new Token(t.Text, t.PreviousSeparator, t.NextSeparator, t.CursorPos)));

        public Tuple<string, int> Combine()
        {
            if (!_tokens.Any())
            {
                return Tuple.Create(string.Empty, 0);
            }

            var builder = new StringBuilder();
            builder.Append(_tokens.First().PreviousSeparator);

            int cursor = 0;
            foreach (var token in _tokens)
            {
                var len = builder.Length;
                builder.Append(token.Text);
                if (token.HasCursor)
                    cursor = len + token.CursorPos;

                builder.Append(token.NextSeparator);
            }

            return Tuple.Create(builder.ToString(), cursor);
        }
    }
}