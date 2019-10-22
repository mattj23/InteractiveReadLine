using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lexer = System.Func<InteractiveReadLine.LineState, InteractiveReadLine.Tokenizing.TokenizedLine>;

namespace InteractiveReadLine.Tokenizing
{
    public static class CommonLexers
    {
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

            if (!tokenized.Any() ||
                (tokenized.Last.IsHidden && tokenized.Last.Cursor == tokenized.Last.Text.Length))
            {
                tokenized.Add(string.Empty, false, 0);
            }

            return tokenized;
        }

    }
}