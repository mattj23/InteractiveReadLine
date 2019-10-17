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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var config = ReadLineConfig.Empty()
                .AddStandardKeys()
                .AddTabAutoComplete()
                .SetAutoCompletion(AutoComplete)
                .SetTokenizer(CommonTokenizers.SplitOnSpaces);

            for (int i = 0; i < 5; i++)
            {
                var provider = new ConsoleReadLine("hello > ");
                var result = provider.ReadLine(config);

                Console.WriteLine(result);
            }
        }

        private static string[] AutoComplete(Tokens tokens)
        {
            var suggestions = new List<string>();
            var options = new string[] {"docker", "docker-compose", "git", "vim", "find", "hello", "grep"};

            if (tokens.CursorToken != null)
                suggestions.AddRange(options.Where(x => x.StartsWith(tokens.CursorToken.Text)));

            return suggestions.ToArray();
        }

    }
}
