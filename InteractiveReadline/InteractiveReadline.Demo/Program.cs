using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                .SetAutoCompletion(AutoComplete)
                .SetTokenizer(CommonTokenizers.SplitOnSpaces);

            while (true)
            {
                _provider = new ConsoleReadLine("prompt > ");
                var result = _provider.ReadLine(config);

                Console.WriteLine(result);

                if (result == "exit")
                    break;
            }
        }

        private static string WriteHelp(Tokens tokens)
        {
            return $"matching commands: {string.Join(", ", AutoComplete(tokens))}";

        }

        private static string[] AutoComplete(Tokens tokens)
        {
            var suggestions = new List<string>();

            if (tokens.CursorToken != null)
                suggestions.AddRange(_options.Where(x => x.StartsWith(tokens.CursorToken.Text)));

            return suggestions.ToArray();
        }

    }
}
