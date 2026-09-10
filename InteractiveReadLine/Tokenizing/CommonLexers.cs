using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lexer = System.Func<InteractiveReadLine.LineState, InteractiveReadLine.Tokenizing.TokenizedLine>;
using RegexList = System.Collections.Generic.List<InteractiveReadLine.Tokenizing.RegexTokenDef>;

namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// Provides ready-made lexers and components for creating custom lexers. A lexer splits a line into tokens
    /// used by auto-completion and token-based formatters.
    /// </summary>
    public static class CommonLexers
    {
        /// <summary>
        /// Creates an empty list of regular-expression token types. Add token patterns to the list, then convert
        /// it to a lexer with ToLexer.
        /// </summary>
        public static RegexList Regex => new RegexList();

        /// <summary>
        /// Adds a new token type to the end of the regular expression token list. The token must begin with
        /// the regular expression start of text character (^) or an ArgumentException will be thrown
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pattern">The regular expression pattern which identifies the token, must start with ^</param>
        /// <param name="typeCode">An optional type code, such that any token matched by this definition will have
        /// this value in the .TypeCode property of the IToken interface</param>
        /// <returns></returns>
        public static RegexList AddTokenType(this RegexList list, string pattern, int typeCode = 0)
        {
            if (!pattern.StartsWith("^"))
                throw new ArgumentException($"Regex token definitions must start with the ^ character, which was not present in the pattern '{pattern}'");

            list.Add(new RegexTokenDef(pattern, typeCode));
            return list;
        }

        /// <summary>
        /// Adds a token type that matches a sequence of non-whitespace characters, which commonly represents
        /// one word.
        /// </summary>
        /// <param name="list">The token type list to add to</param>
        /// <param name="typeCode">A code identifying this token type in the resulting tokens</param>
        /// <returns>The same list, for chaining</returns>
        public static RegexList AddAnyNonWhitespace(this RegexList list, int typeCode = 0) =>
            list.AddTokenType(@"^\S+", typeCode);

        /// <summary>
        /// Adds a token type that matches a double-quoted string and honors backslash escapes, so a quoted
        /// phrase is treated as one token rather than split on its spaces.
        /// </summary>
        /// <param name="list">The token type list to add to</param>
        /// <param name="typeCode">A code identifying this token type in the resulting tokens</param>
        /// <returns>The same list, for chaining</returns>
        public static RegexList AddDoubleQuoteStringLiterals(this RegexList list, int typeCode = 0) =>
            list.AddTokenType(@"^""(?:[^""\\]|\\.)*""", typeCode);

        /// <summary>
        /// Adds a token type that matches a single-quoted string and honors backslash escapes.
        /// </summary>
        /// <param name="list">The token type list to add to</param>
        /// <param name="typeCode">A code identifying this token type in the resulting tokens</param>
        /// <returns>The same list, for chaining</returns>
        public static RegexList AddSingleQuoteStringLiterals(this RegexList list, int typeCode = 0) =>
            list.AddTokenType(@"^'(?:[^'\\]|\\.)*'", typeCode);

        /// <summary>
        /// From a list of RegexTokenDefs, produce a function that will tokenize a LineState according to
        /// the specified token definitions in the order they were provided. Effectively this renders a list
        /// of regular expression token definitions into a functioning lexer.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Lexer ToLexer(this RegexList list)
        {
            if (!list.Any())
                throw new ArgumentException("The RegexTokenDefs list must not be empty");

            return lineState =>
            {
   
                var tokenized = new TokenizedLine();

                var ignored = new StringBuilder();
                for (int i = 0; i < lineState.Text.Length; i++)
                {
                    var substring = lineState.Text.Substring(i);

                    bool hasMatch = false;

                    foreach (var tokenDef in list)
                    {
                        var match = tokenDef.Regex.Match(substring);
                        if (match.Success)
                        {
                            if (ignored.Length > 0)
                            {
                                tokenized.Add(ignored.ToString(), true, null);
                                ignored.Clear();
                            }

                            tokenized.Add(match.Value, false, null, tokenDef.TypeCode);
                            i += match.Value.Length - 1;
                            hasMatch = true;
                            break;
                        }
                            
                    }
                    if (!hasMatch)
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

                var last = tokenized.Last;
                if (last == null ||
                    (last.IsHidden && last.Cursor == last.Text.Length))
                {
                    tokenized.Add(string.Empty, false, 0);
                }

                return tokenized;
            };
        }

        /// <summary>
        /// Gets a lexer that splits a line into words at whitespace boundaries for use with auto-completion.
        /// </summary>
        public static Lexer SplitOnWhitespace => Regex.AddAnyNonWhitespace().ToLexer();

    }
}
