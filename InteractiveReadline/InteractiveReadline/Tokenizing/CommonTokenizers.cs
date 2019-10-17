using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}