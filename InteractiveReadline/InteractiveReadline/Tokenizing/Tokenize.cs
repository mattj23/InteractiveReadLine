using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// A workspace to tokenize strings that enforces strict guarantees about what is allowed. Namely,
    /// all characters in the string must be assigned to either a token or a separator. Contiguous token characters
    /// are consolidated into a single token, all characters between them are separators.
    /// </summary>
    public class Tokenize
    {
        public Tokenize(string text, int cursor)
        {
            Text = text;
            Cursor = cursor;
            IsToken = new bool[Text.Length];
        }

        public int Cursor { get; }

        public string Text { get; }

        public bool[] IsToken { get; }

        /// <summary>
        /// Performs the tokenization of the given text
        /// </summary>
        /// <returns></returns>
        public Tokens Result()
        {
            var tokens = new Tokens();

            var prevSep = new StringBuilder();
            var thisToken = new StringBuilder();
            var nextSep = new StringBuilder();

            /* Segmentation of the bool array into separators and tokens

            The segmentation task consists of marching through the bool array and adding characters to one of the
            string builders based on the condition of the last character.

            Starting at the beginning of the array, all builders are empty.  If the first character is a separator,
            we add it to the previous separator builder, then the next character, and so on.

            When we hit the first token character, we begin to add to the token builder.  From then on, no characters
            will be added to the previous builder again.  When we hit the end of the token characters, we begin 
            to add to the next builder.  

            When we hit the next token characters, we will create the first token with its previous and next 
            separators. The previous builder takes the value of the next builder, and the token and next builders
            are cleared.

            When we get to the end of the text, we add the final token and its separators.

            If the cursor is beyond the length of the text, AND the last separator is not empty, we add one final empty
            token. This is important for knowing if the cursor is past the last token.

             
             */

            bool isPrev = true;
            int tokenStart = 0;
            int tokenCursor = Int32.MinValue;

            // Iterating through all of the characters in the text
            for (int i = 0; i < Text.Length; i++)
            {
                if (i == 0)
                {
                    // There's a special case for the first element, which cannot be allowed to perform a 
                    // look-behind. For the first element the choice is very simple, either add to prev 
                    // or add to the token. If we add to the token, prev will never be added to.
                    if (IsToken[i])
                    {
                        isPrev = false;
                        tokenStart = i;
                        thisToken.Append(Text[i]);
                    }
                    else
                    {
                        prevSep.Append(Text[i]);
                    }
                }
                else
                {
                    // If the element is not the first, we will need to check for one of the transition states. If 
                    // we are switching from non-token to token, we need to add the built elements to the token list,
                    // move the next sep to the prev sep, clear the existing token and next sep, then add the current
                    // character to the newly cleared token.
                    if (IsToken[i] && !IsToken[i-1])
                    {
                        if (!isPrev)
                        {
                            // This is the transition state described above
                            tokens.Add(thisToken.ToString(), nextSep.ToString(), prevSep.ToString(), tokenCursor);
                            prevSep.Clear().Append(nextSep);
                            nextSep.Clear();
                            thisToken.Clear();
                            tokenStart = i;
                            tokenCursor = Int32.MinValue;
                        }
                        else
                        {
                            isPrev = false;
                            
                            // Here we need to handle the case that the cursor was in the pre-first-token text
                            if (tokenCursor > Int32.MinValue)
                            {
                                // We are currently on (i) the first token character. The cursor position will
                                // be negative from this character
                                tokenCursor = tokenCursor - i;
                            }

                            tokenStart = i;
                        }
                    }

                    if (IsToken[i])
                    {
                        thisToken.Append(Text[i]);
                    }
                    else if (isPrev)
                    {
                        prevSep.Append(Text[i]);
                    }
                    else
                    {
                        nextSep.Append(Text[i]);
                    }

                }

                if (i == Cursor)
                    tokenCursor = i - tokenStart;

            }

            // Now that we've reached the end, we add the final token
            if (isPrev)
            {
                tokenCursor = tokenCursor > Int32.MinValue ? tokenCursor - Text.Length : 0;
            }
            else
            {
                if (Cursor == Text.Length)
                    tokenCursor = Text.Length - tokenStart;
            }
            tokens.Add(thisToken.ToString(), nextSep.ToString(), prevSep.ToString(), tokenCursor);

            // If the cursor is beyond the length of the text, and the last separator wasn't empty, we add
            // a final empty token
            if (Cursor >= Text.Length && !string.IsNullOrEmpty(tokens.Last().NextSeparator.Text))
            {
                tokens.Add(string.Empty, null, null, 0);
            }

            return tokens;
        }
        
    }
}