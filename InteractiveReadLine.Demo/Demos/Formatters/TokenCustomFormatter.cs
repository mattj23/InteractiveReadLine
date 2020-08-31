using System;
using System.Text.RegularExpressions;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class TokenCustomFormatter : IDemo
    {
        public string Description => "A custom formatter which receives a TokenizedLine";
        
        public void Action()
        {
            Console.WriteLine("The following formatter shows all valid hexadecimal words in uppercase,");
            Console.WriteLine("cyan letters. Note that the actual text buffer is not being changed, it");
            Console.WriteLine("is simply altering how the characters are displayed.\n");

            /* This example shows a formatter which receives a TokenizedLine instead of the simpler
             LineState object. Typically you would create a TokenizedLine formatter if you're using a
             lexer for autocompletion, or because you need to tokenize the line anyway according to some
             grammatical rules and you want to use the formatter to provide some level of validation to
             the user.
             
             For simple formatters, like those that create fixed prompts or simple operations on text,
             the TokenizedLine formatter is excessive, since the lexer will need to be called every time
             the user presses a key.  Also, in order to use a TokenizedLine formatter the configuration 
             will need to be provided with a lexer.
             
             In the following example, we use one of the prebuilt lexers (CommonLexers.SplitOnWhitespace)
             to separate non-whitespace text chunks into tokens, and then we have a regular expression 
             that matches characters a-f and digits 0-9 in order to identify valid hexadecimal tokens.
             
             We then turn those tokens cyan and convert their alphabetical characters to uppercase for
             the purpose of displaying them.  Again, keep in mind that the actual text buffer is not
             being altered, only the formatting.
             */
            var pattern = new Regex(@"^[0-9a-fA-F]+$");
            
            var formatter = new Func<TokenizedLine, LineDisplayState>(tk =>
            {
                FormattedText output = string.Empty;
                foreach (var token in tk)
                {
                    if (pattern.IsMatch(token.Text))
                        output += new FormattedText(token.Text.ToUpper(), ConsoleColor.Cyan);
                    else
                        output += token.Text;
                }
                
                return new LineDisplayState("try it: ", output, string.Empty, tk.Cursor);
            });

            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetFormatter(formatter);

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}