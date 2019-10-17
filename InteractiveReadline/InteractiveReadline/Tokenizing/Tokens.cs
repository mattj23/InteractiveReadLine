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
        private readonly List<Separator> _separators;

        public Tokens()
        {
            _tokens = new List<Token>();
            _separators = new List<Separator>();
        }

        public int Count => _tokens.Count;

        public Token this[int index] => _tokens[index];

        public Token CursorToken => _tokens.FirstOrDefault(t => t.HasCursor);

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string tokenText, string nextSep, string prevSep = null, int cursorPos = Int32.MinValue)
        {
            Separator prevSeparator = null;
            if (!_tokens.Any())
            {
                // This is the first token, so it must take the previous separator and set it
                prevSeparator = new Separator(prevSep ?? string.Empty);
                _separators.Add(prevSeparator);
            }

            var nextSeparator = new Separator(nextSep ?? string.Empty);
            _separators.Add(nextSeparator);

            var token = new Token(tokenText ?? string.Empty, nextSeparator, prevSeparator, cursorPos);

            // If there's a previous token, we link it to this one
            _tokens.LastOrDefault()?.LinkToNext(token);

            _tokens.Add(token);
        }

        // public Tokens Clone() => new Tokens(_tokens.Select(t => new Token(t.Text, t.PreviousSeparator, t.NextSeparator, t.CursorPos)));

        public Tuple<string, int> Combine()
        {
            if (!_tokens.Any())
            {
                return Tuple.Create(string.Empty, 0);
            }

            var builder = new StringBuilder();
            builder.Append(_tokens.First().PrevSeparator.Text);

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