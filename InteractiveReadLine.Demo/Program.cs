using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using InteractiveReadLine.Demo.Demos;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        private static ConsoleReadLine _provider;
        private static string[] _options;

        private static DemoNode _demoHome;


        static void Main(string[] args)
        {
            Console.WriteLine("InteractiveReadLine Demo Program");
            Console.WriteLine("\nThis executable is a demonstration to showcase some of the features of this library.\nYou can exit at any time with the command 'exit'.''");

            _demoHome = new DemoNode(null, "Demo Home");
            _demoHome.AddChild("basic", new BasicConfig());
            var keyNode = _demoHome.AddChild("keys", "Key Behaviors and Customization");

        }

        private static void Home()
        {

        }

        private static void BasicSettings()
        {

        }


        private static string WriteHelp(TokenizedLine tokens)
        {
            return $"matching commands: {string.Join(", ", AutoComplete(tokens))}";

        }

        private static LineDisplayState Formatter(TokenizedLine tokenized)
        {
            var prompt = new FormattedText("(prompt)> ");
            var nonHidden = tokenized.Where(x => !x.IsHidden).ToArray();

            if (nonHidden.Length % 2 == 0)
                prompt.SetForeground(ConsoleColor.Red);
            else 
                prompt.SetForeground(ConsoleColor.Blue);

            FormattedText output = string.Empty;
            int color = 0;
            foreach (var token in tokenized)
            {
                var text = new FormattedText(token.Text);

                if (!token.IsHidden)
                {
                    color = (++color) % 3;
                    if (color == 1)
                        text.SetForeground(ConsoleColor.Green);
                    else if (color == 2)
                        text.SetBackground(ConsoleColor.DarkBlue);
                    else
                        text.SetForeground(ConsoleColor.Blue);
                }

                output += text;
            }

            var suffix = new FormattedText($"   [{tokenized.Count} tokens]");
            suffix.SetForeground(ConsoleColor.Gray);
            return new LineDisplayState(prompt, output, suffix, tokenized.Cursor);
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
