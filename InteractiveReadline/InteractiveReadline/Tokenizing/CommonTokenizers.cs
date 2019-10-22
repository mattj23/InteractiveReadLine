using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tokenizer = System.Func<InteractiveReadLine.Tokenizing.Tokenize, InteractiveReadLine.Tokenizing.Tokens>;

namespace InteractiveReadLine.Tokenizing
{
    public static class CommonTokenizers
    {
        public static Tokenizer SplitOnCharacters(params char[] c)
        {
            return tokenize =>
            {
                for (int i = 0; i < tokenize.Text.Length; i++)
                {
                    tokenize.IsToken[i] = !c.Contains(tokenize.Text[i]);
                }

                return tokenize.Result();
            };
        }

        public static Tokenizer SplitOnSpaces => SplitOnCharacters(' ');

        public static Tokenizer SplitOnCommas => SplitOnCharacters(',');

        public static Tokenizer SplitOnPunctuation => SplitOnCharacters(' ', ',', '.', '!', '?', ':', ';');

        public static TokenizedLine SplitOnWhitespace(LineState lineState)
        {
            var regex = new Regex(@"^\S+");
            var tokenized = new TokenizedLine();

            var ignored = new StringBuilder();
            for (int i = 0; i < lineState.Text.Length; i++)
            {
                var substring = lineState.Text.Substring(i);

                var match = regex.Match(substring);
                if (match.Success)
                {
                    if (ignored.Length > 0)
                    {
                        tokenized.Add(ignored.ToString(), true);
                        ignored.Clear();
                    }

                    tokenized.Add(match.Value, false);
                    i += match.Value.Length - 1;
                }
                else
                {
                    ignored.Append(lineState.Text[i]);
                }
            }

            if (ignored.Length > 0)
            {
                tokenized.Add(ignored.ToString(), true);
                ignored.Clear();
            }

            tokenized.Cursor = lineState.Cursor;

            if (tokenized.Last.IsHidden && tokenized.Last.Cursor == tokenized.Last.Text.Length)
            {
                tokenized.Add(string.Empty, false, 0);
            }

            return tokenized;
        }

    }
}