using System;
using System.Runtime.InteropServices;
using InteractiveReadLine.Behaviors;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WindowHeight = 5;
            Console.BufferHeight = 5;

            var config = new ReadLineConfig().AddStandardKeys();

            for (int i = 0; i < 5; i++)
            {
                var provider = new ConsoleReadLine("hello > ");
                var result = provider.ReadLine(config);

                Console.WriteLine(result);
            }
        }
    }
}
