using System;
using System.Runtime.InteropServices;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WindowHeight = 5;
            Console.BufferHeight = 5;

            for (int i = 0; i < 5; i++)
            {
                var provider = new ConsoleReadLine("hello > ");
                var result = provider.ReadLine(null);

                Console.WriteLine(result);
            }
        }
    }
}
