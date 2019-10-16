using System;
using System.Runtime.InteropServices;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var provider = new ConsoleReadLine("hello > ");
            var handler = new InputHandler(provider);
            var result = handler.ReadLine();
            handler.Dispose();

            Console.WriteLine(result);
        }
    }
}
