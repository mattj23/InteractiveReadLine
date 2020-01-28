using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using InteractiveReadLine.Demo.Demos;
using InteractiveReadLine.Demo.Demos.Keys;
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
        private static DemoNode _activeNode;


        static void Main(string[] args)
        {
            Console.WriteLine("InteractiveReadLine Demo Program");
            Console.WriteLine("\nThis executable is a demonstration to showcase some of the features of this library.\nYou can exit at any time with the command 'exit'.''");
            Console.WriteLine();

            _demoHome = new DemoNode(null, "home", "Demo Home");
            _demoHome.AddChild("basic", new BasicConfig());
            var keyNode = _demoHome.AddChild("keys", "Key Behaviors and Customization");
            keyNode.AddChild("bare", new BareKeys());

            _activeNode = _demoHome;
            bool isRunning = true;

            while (isRunning)
            {
                var options = new Dictionary<string, Action>
                {
                    {"exit", () => isRunning = false },
                    {"home", () => _activeNode = _demoHome }
                };

                if (_activeNode.Demo != null)
                {
                    // This is a demo node
                    options.Add("run", () => _activeNode.Demo.Action());
                }
                else
                {
                    // This is a menu node, show the node and its first level children
                    Console.WriteLine($" {_activeNode.Description}");

                    var names = _activeNode.OrderedChildKeys;
                    var padding = names.Select(n => " ").ToArray();
                    var descriptions = names.Select(k => _activeNode.Children[k].Description).ToArray();
                    Console.WriteLine(FormatTable(false, padding, names, descriptions));

                    foreach (var name in names)
                    {
                        options.Add(name, () => _activeNode = _activeNode.Children[name]);
                    }

                }

                var config = ReadLineConfig.Basic
                    .AddCtrlNavKeys()
                    .AddTabAutoComplete()
                    .SetLexer(CommonLexers.SplitOnWhitespace)
                    .SetAutoCompletion(t => options.Keys.Where(o => o.StartsWith(t.CursorToken.Text)).ToArray())
                    .SetFormatter(NodeFormatter(options.Keys.ToArray()));

                var result = ConsoleReadLine.ReadLine(config);

                if (options.ContainsKey(result))
                {
                    options[result].Invoke();
                }
            }
        }

        private static Func<TokenizedLine, LineDisplayState> NodeFormatter(IReadOnlyList<string> options)
        {
            var path = _activeNode.Path;

            return line =>
            {
                var prefix = new FormattedText($"{path} > ");

                FormattedText suffix = "";
                if (line.FirstNonHidden.Text == "exit")
                    suffix = new FormattedText(" [exits the demo program]", ConsoleColor.Red);
                else if (line.FirstNonHidden.Text == "home")
                    suffix = new FormattedText(" [returns to demo root]", ConsoleColor.Blue);

                return new LineDisplayState(prefix, line.Text, suffix, line.Cursor);

            };
        }


        public static string FormatTable(bool header, params IReadOnlyList<string>[] columns)
        {
            var rows = new List<string>();
            var colWidths = columns.Select(c => c.Max(x => x.Length)).ToArray();

            for (int i = 0; i < columns[0].Count; i++)
            {
                var cols = new List<string>();
                for (int j = 0; j < columns.Length; j++)
                {
                    cols.Add(columns[j][i] + new string(' ', colWidths[j] - columns[j][i].Length + 1));
                }
                rows.Add(string.Join("  ", cols));
            }

            // Find the longest row, and insert a horizontal rule after the first row
            var longest = rows.Select(x => x.Length).Max();

            if (header)
                rows.Insert(1, new string('-', longest));

            return string.Join("\n", rows);

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
