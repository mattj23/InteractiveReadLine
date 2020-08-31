using System;
using System.Linq;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class Hunter2 : IDemo
    {
        public string Description => "Performs the infamous bash.org 'hunter2' prank";
        
        public void Action()
        {
            Console.WriteLine("The 'hunter2' prank (http://bash.org/?244321), try entering");
            Console.WriteLine("both the 'password' and the stars\n");

            var formatter = new Func<LineState, LineDisplayState>(ls =>
            {
                var pranked = string.Join("*******", ls.Text.Split("hunter2")
                    .Select(segment => string.Join("hunter2", segment.Split("*******"))));
                
                return new LineDisplayState("#AzureDiamond> ", pranked, string.Empty, ls.Cursor);
            });

            var config = ReadLineConfig.Basic.SetFormatter(formatter);

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}