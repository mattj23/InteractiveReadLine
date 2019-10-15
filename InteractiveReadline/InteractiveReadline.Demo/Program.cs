using System;

namespace InteractiveReadLine.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var testHandler = new ReadLineHandler(new SystemReadLine());

            var test = testHandler.ReadLine("hello > ");
        }
    }
}
