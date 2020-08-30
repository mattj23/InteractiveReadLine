using System;
using System.Linq;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.Demo.Demos
{
    public class ComplexConfig : IDemo
    {
        public string Description => "Shows a complex configuration";
        
        public void Action()
        {
            /* This is a fairly complex example that incorporates several different aspects of the library into
             a single configuration.  Below you will find a simple regex based lexer with a custom token, an 
             autocomplete feature with a list of words, and a custom key behavior that writes out a message about
             the text that has been entered.
             
             Each of these individual features represent an simple example of their respective functionality, but
             this example is to show how they all come together.
             */
            
            Console.WriteLine("This shows an example of a more complex configuration, including autocomplete,");
            Console.WriteLine("a '?' help option, some formatting, and lexing. Look at the source code at the");
            Console.WriteLine("github link for documentation.\n");

            /* A lexer is a function which is given a LineState* and returns a TokenizedLine object. You are free to
             build your own lexer and use the TokenizedLine's "Add" method to put together the various tokens. However, 
             there is already a simple regular expression based tool which allows you to add regex based token 
             identifiers together in an ordered list and then convert it into a lexer that can be used by the 
             configuration. It performs adequately for the amount of text typically involved in readline usage.
             
             The following is an example of assembling a regex lexer through this tool. The order is important because
             it indicates the priority of different token types. "CommonLexers.Regex" is actually a list type under
             the hood, so be sure to convert it to a Func<LineState, TokenizedLine> with the "ToLexer()" method. 
             
             Note:  a "LineState" is text and cursor position, and is important so the ReadLine object knows which
                    token actively has the cursor in it. 
             */
            var lexer = CommonLexers.Regex
                .AddSingleQuoteStringLiterals(1)
                .AddDoubleQuoteStringLiterals(2)
                .AddTokenType(@"^\d+(?=\s|$)", 3)  // this looks for 1 or more digits followed by a whitespace
                .AddAnyNonWhitespace()
                .ToLexer();
            
            /* Here we create the configuration for the ReadLine operation.  Note that we are adding a custom behavior
             for the ? key, which will now trigger an action that prints out a line of text above the ReadLine in the
             console. A KeyBehavior is simply any Action which accepts a IKeyBehaviorTarget as its argument. See the
             documentation and/or examples for the full capabilities of key behaviors, but here we are using yet another 
             built in helper (WriteMessageFromTokens) to compose this help printing action from a simpler function.
             */
            var config = ReadLineConfig.Empty
                .AddStandardKeys()
                .AddTabAutoComplete()
                .AddKeyBehavior('?', HelpAction) 
                .SetFormatter(Formatter)
                .SetAutoCompletion(AutoComplete)
                .SetLexer(lexer);

            // Perform the interactive read line from the console
            string result = ConsoleReadLine.ReadLine(config);
            
            // Note: If you wanted to use the lexer to tokenize the result of the ReadLine operation, there is nothing
            // stopping you from using it manually yourself.  A lexer will accept a bare string because a string
            // can implicitly be converted to a LineState by assuming the cursor is at the 0 position.
            var tokenized = lexer(result);
        }

        // These are the words which this example will autocomplete
        private readonly string[] _autoCompleteWords = {"complex", "configuration", "readline", "example", "control"};

        /* This is a formatter.  A formatter is any function which takes *either* a TokenizedLine or a LineState and
         returns a LineDisplayState.  If you set the ReadLine configuration to use a TokenizedLine the handler must be
         given a lexer as well, otherwise the formatter which simply receives the LineState must be used. The reason
         for this distinction is to allow the handler to run the lexer only one time internally if both a formatter and
         an autocompletion service is used.
         
         A LineDisplayState consists of three pieces of FormattedText (FormattedText is text with both a foreground and
         background ConsoleColor which can be set individually per character).  These three pieces are a prefix, a body,
         and a suffix.  The LineDisplayState also has a cursor position, which is an offset from the first character of
         the body.
         
         The built-in formatters in CommonFormatters are composable, in that you can create functions which take another
         formatter as an argument and return a formatter that combines the behavior of the two, perhaps adding a prefix
         or a suffix (the WithFixedPrompt extension method works this way), but here we'll just write a simple formatter
         that handles prefix, suffix, and body all by itself.
         
         This is also a demonstration of how a formatter doesn't need to be a Func<...> and can be a class method, for
         example, and so can be attached to state accessible within the class.
         */
        private LineDisplayState Formatter(TokenizedLine line)
        {
            // We will build the body output in this
            FormattedText output = string.Empty;

            /* We are going to do a few things to the body text. First, any string literal token will be turned cyan or
             yellow, depending on whether it uses single or double quotes. Any integer token becomes red, any 
             autocomplete term becomes magenta.  Finally, the token with the cursor under it gets a dark blue 
             background.*/
            foreach (var token in line)
            {
                var text = new FormattedText(token.Text);
                switch (token.TypeCode)
                {
                    case 1:
                        text.SetForeground(ConsoleColor.Cyan);
                        break;
                    case 2:
                        text.SetForeground(ConsoleColor.Yellow);
                        break;
                    case 3:
                        text.SetForeground(ConsoleColor.Red);
                        break;
                }

                if (_autoCompleteWords.Contains(token.Text))
                {
                    text.SetForeground(ConsoleColor.Magenta);
                }
                
                if (token.Cursor != null)
                {
                    text.SetBackground(ConsoleColor.DarkBlue);
                }
                
                output += text;
            }
            
            // The prefix will simply be a fixed prompt with green text
            var prefix = new FormattedText("Enter Text > ", ConsoleColor.Green);
            
            // There will not be a suffix, so we can put in an empty string. We won't be adjusting the visual
            // position of the cursor, so we can copy it over directly from the input.
            return new LineDisplayState(prefix, output, string.Empty, line.Cursor);
        }

        /* Autocomplete works by a function which takes a TokenizedLine and returns an array of string options for the
         token under the cursor. It is called once when the user invokes the autocomplete feature (typically bound to
         the tab key) and the token under the cursor is replaced with the first element of the array.  Further 
         invocations will not call this method again unless the text changes in some way, but will rather cycle through
         the array of given options.
         
         This allows for both a cycling (powershell) type autocomplete by returning an array with many elements or a 
         complete-only-to-unique style (bash) by returning an array with a single element.*/
        private string[] AutoComplete(TokenizedLine line)
        {
            // In this case we're simply going to return the autocomplete words which start with the text in the token
            // containing the cursor, but there's no reason we couldn't look backwards at the previous token(s) to 
            // consider their context.
            return _autoCompleteWords.Where(s => s.StartsWith(line.CursorToken.Text)).ToArray();
        }
        
        /* While not a core feature of this library, this is an interesting example of flexible composition of behavior.
         On the Cisco command line there is a feature where pressing the ? key will show the different context 
         dependant words which could come next.  This feature mimics that behavior.
         
         Key behaviors are Actions which take the IKeyBehaviorTarget, which is an exposed set of features on the 
         ReadLine's core handler object, allowing the behavior to mutate the state or invoke other actions.  In fact all
         behavior, including the most basic inserting of characters into the text buffer, is performed with this 
         mechanism.
         
         Here, we're going to create a custom action which writes out any autocomplete options for the current token
         under text. It uses the 
         */
        private void HelpAction(IKeyBehaviorTarget target)
        {
            // We'll simply reuse the AutoComplete method rather than re-implementing it here. One of the available
            // methods on the key behavior target is running the lexer.  Be aware that if no lexer was given to the
            // configuration, this will return null.
            var line = target.GetTextTokens();
            var options = AutoComplete(line);

            var text = "Complete with: " + string.Join(", ", options);
            
            // InsertText is an exposed method on the handler which writes a string into the console in the spot where
            // the line of input is happening and then redisplays interactive readline below it.  It can be used to 
            // insert asynchronous messages into the console while a readline operation is happening.
            target.InsertText(text);
        }
        
        
    }
}