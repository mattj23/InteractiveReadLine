using System;
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
            Console.WindowHeight = 5;
            Console.BufferHeight = 5;

            var config = ReadLineConfig.Empty()
                .AddStandardKeys()
                .SetTokenizer(CommonTokenizers.SplitOnSpaces);

            for (int i = 0; i < 5; i++)
            {
                var provider = new ConsoleReadLine("hello > ");
                var result = provider.ReadLine(config);

                Console.WriteLine(result);
            }
        }
    }
}
