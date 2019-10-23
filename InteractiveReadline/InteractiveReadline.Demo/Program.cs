using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        private static ConsoleReadLine _provider;
        private static string[] _options;

        static void Main(string[] args)
        {
            
            _options = new string[] {"docker", "docker-compose", "git", "vim", "find", "hello", "grep", "exit"};
            Console.WriteLine("Hello World!");

            var config = ReadLineConfig.Empty()
                .AddStandardKeys()
                .AddTabAutoComplete()
                .AddKeyBehavior('?', CommonBehaviors.WriteMessageFromTokens(WriteHelp)) 
                .SetFormatter(CommonFormatters.FixedPrompt("prompt > "))
                .SetAutoCompletion(AutoComplete)
                .SetLexer(CommonLexers.SplitOnWhitespace);

            while (true)
            {
                var result = ConsoleReadLine.ReadLine(config);

                Console.WriteLine(result);

                if (result == "exit")
                    break;
            }
        }

        private static string WriteHelp(TokenizedLine tokens)
        {
            return $"matching commands: {string.Join(", ", AutoComplete(tokens))}";

        }

        private static string[] AutoComplete(TokenizedLine tokens)
        {
            var suggestions = new List<string>();

            if (tokens.CursorToken != null)
                suggestions.AddRange(_options.Where(x => x.StartsWith(tokens.CursorToken.Text)));

            return suggestions.ToArray();
        }

    }
}
