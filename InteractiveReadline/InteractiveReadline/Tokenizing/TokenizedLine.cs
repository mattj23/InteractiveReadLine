using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

namespace InteractiveReadLine.Tokenizing
{
    public class TokenizedLine : IReadOnlyList<IToken>
    {
        private readonly List<Token> _tokens;
        private int _cursor;

        public TokenizedLine()
        {
            _tokens = new List<Token>();

        }

        public IToken First => _tokens.FirstOrDefault();
        public IToken Last => _tokens.LastOrDefault();

        public string Text => Token.BuildText(_tokens);

        public IEnumerator<IToken> GetEnumerator() => _tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _tokens.Count;

        public IToken this[int index] => _tokens[index];

        public IToken CursorToken => _tokens.FirstOrDefault(x => x.Cursor != null);

        public int CursorTokenIndex => _tokens.IndexOf(_tokens.FirstOrDefault(x => x.Cursor != null));

        public int Cursor
        {
            get => _cursor;
            set
            {
                _cursor = value;
                
                int textLen = this.Text.Length;
                if (value > textLen)
                    _cursor = textLen;
            }
        }

        public void Add(string text, bool isHidden, int? cursor=null)
        {
            var newToken = new Token(text, isHidden, this);

            if (_tokens.Any())
            {
                _tokens.Last().Next = newToken;
                newToken.Previous = _tokens.Last();
            }

            _tokens.Add(newToken);

            if (cursor != null)
                newToken.Cursor = cursor;
        }


        private class Token : IToken
        {
            private readonly TokenizedLine _parent;
            private string _text;

            public Token(string text, bool isHidden, TokenizedLine parent)
            {
                this.Text = text;
                IsHidden = isHidden;
                _parent = parent;
            }

            public string Text
            {
                get => _text;
                set => _text = value;
            }

            public int? Cursor
            {
                get
                {
                    int lengthBefore = this.TextBefore().Length;
                    if (_parent.Cursor < lengthBefore)
                        return null;

                    int offset = _parent.Cursor - lengthBefore;

                    if (offset == Text.Length && Next?.IsHidden == false)
                    {
                        return null;
                    }

                    if (offset <= Text.Length)
                        return offset;
                    return null;
                }
                set
                {
                    if (value == null || value < 0 || value > Text.Length)
                        return;

                    _parent.Cursor = this.TextBefore().Length + (int) value;
                }
            }

            public Token Next { get; set; }
            public Token Previous { get; set; }
            
            public IToken PreviousNotHidden => this.Previous?.ThisOrPrevIfHidden();
            public IToken NextNotHidden => this.Next?.ThisOrNextIfHidden();

            IToken IToken.Next => this.Next;

            IToken IToken.Previous => this.Previous;

            public Token First => this.Previous == null ? this : this.Previous.First;

            public bool IsHidden { get; set; }

            private Token[] TokensBefore()
            {
                var tokens = new List<Token>();
                var first = this.First;

                var pointer = first;
                while (pointer != this)
                {
                    tokens.Add(pointer);
                    pointer = pointer.Next;
                }

                return tokens.ToArray();
            }


            private string TextBefore()
            {
                return BuildText(this.TokensBefore());
            }

            private Token ThisOrNextIfHidden()
            {
                if (this.IsHidden)
                    return this.Next?.ThisOrNextIfHidden();
                else
                    return this;
            }

            private Token ThisOrPrevIfHidden()
            {
                if (this.IsHidden)
                    return this.Previous?.ThisOrPrevIfHidden();
                else
                    return this;
            }

            public static string BuildText(IEnumerable<Token> tokens)
            {
                var builder = new StringBuilder();
                foreach (var token in tokens)
                {
                    builder.Append(token.Text);
                }

                return builder.ToString();
            }
        }
    }
}