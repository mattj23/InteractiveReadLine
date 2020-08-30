using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using InteractiveReadLine.Demo.Demos;
using InteractiveReadLine.Demo.Demos.Formatters;
using InteractiveReadLine.Demo.Demos.Keys;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;
using Microsoft.VisualBasic.CompilerServices;

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

            var formatNode = _demoHome.AddChild("formatters", "Display Formatting and Customization");
            formatNode.AddChild("fixed-prompt", new FixedPrompt());
            formatNode.AddChild("passwd-stars", new PasswordStarPrompt());
            formatNode.AddChild("passwd-bars", new PasswordBarPrompt());

            _activeNode = _demoHome;
            bool isRunning = true;

            while (isRunning)
            {
                var options = new OptionSet();
                Console.Clear();

                if (_activeNode.Demo != null)
                {
                    // This is a demo node
                    Console.WriteLine($"Demo: {_activeNode.Name} ({_activeNode.Description})");
                    options.AddToStart("run", "Run this demo", () =>
                    {
                        Console.Clear();
                        _activeNode.Demo.Action();
                    });
                }
                else
                {
                    // This is a menu node, show the node and its first level children
                    Console.WriteLine($" {_activeNode.Description}");

                    var names = _activeNode.OrderedChildKeys;
                    foreach (var name in names)
                    {
                        options.Add(name, _activeNode.Children[name].Description, () => _activeNode = _activeNode.Children[name]);
                    }
                }
                
                options.AddSpace();

                if (_activeNode.Parent != null)
                    options.Add("back", "Go back", () => _activeNode = _activeNode.Parent);
                
                options.Add("home", "Return to demo home", () => _activeNode = _demoHome);
                options.Add("exit", "Exit the demo program", () => isRunning = false);
                
                var padding = options.Keys.Select(n => " ").ToArray();
                if (_activeNode.Demo != null)
                {
                    var link = GetGithubLink(_activeNode.Demo);
                    if (link != null)
                        Console.WriteLine($" * {link}");
                }
                Console.WriteLine(FormatTable(false, padding, options.Keys, options.Descriptions));

                var config = ReadLineConfig.Basic
                    .AddCtrlNavKeys()
                    .AddTabAutoComplete()
                    .SetLexer(CommonLexers.SplitOnWhitespace)
                    .SetAutoCompletion(t => options.NonBlankKeys.Where(o => o.StartsWith(t.CursorToken.Text)).ToArray())
                    .SetFormatter(NodeFormatter(options.NonBlankKeys.ToArray()));

                var result = ConsoleReadLine.ReadLine(config);

                if (options.ContainsKey(result))
                {
                    options.GetAction(result).Invoke();
                }
            }
        }

        private static string GetGithubLink(IDemo demo)
        {
            const string rootUrl = "https://github.com/mattj23/InteractiveReadLine/tree/master/InteractiveReadLine.Demo/";
            var typeInfo = demo.GetType();
            var path = typeInfo.FullName?.Replace("InteractiveReadLine.Demo.", "").Split(".");
            if (path != null)
            {
                return rootUrl + string.Join("/", path) + ".cs";
            }

            return null;
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
                else if (line.FirstNonHidden.Text == "back" && _activeNode.Parent != null)
                    suffix = new FormattedText($" [return to {_activeNode.Parent.Name}]", ConsoleColor.Blue);

                return new LineDisplayState(prefix, line.Text, suffix, line.Cursor);

            };
        }


        private static string FormatTable(bool header, params IReadOnlyList<string>[] columns)
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

    }
}
