using System;
using InteractiveReadLine.KeyBehaviors;

namespace InteractiveReadLine.Demo.Demos.Keys
{
    public class BareKeys : IDemo
    {
        public string Description => "Shows an almost bare key configuration";

        public void Action()
        {
            Console.WriteLine("This is a bare key configuration with only the enter key and default insert behavior.\n");

            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonKeyBehaviors.InsertCharacter)
                .AddEnterToFinish();

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}