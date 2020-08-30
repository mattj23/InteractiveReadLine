using System;
using System.Linq;
using InteractiveReadLine.KeyBehaviors;

namespace InteractiveReadLine.Demo.Demos.Keys
{
    public class CustomInsertCharacter : IDemo
    {
        public string Description => "Shows the default insert-key behavior modified to ignore non-numbers";
        
        public void Action()
        {
            Console.WriteLine("Rather than the typical default key behavior which inserts the received key character,");
            Console.WriteLine("this configuration will only insert the key if the character is a digit.\n");
            
            /* This configuration inserts a custom Action into the spot for the default key behavior.  Rather than the
             normal standard key configuration which includes a default insert action, this one has an insert action
             that first checks if the received character is a digit.
             
             Here I construct this key behavior as a lambda, but any function/method which takes an IKeyBehaviorTarget 
             as its single argument will work.
             */
            
            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(target =>
                {
                    // target is an IKeyBehaviorTarget
                    if (char.IsDigit(target.ReceivedKey.KeyChar))
                    {
                        // here we add the character to the target's text buffer at the current cursor position and 
                        // then advance the cursor position by a single place
                        target.TextBuffer.Insert(target.CursorPosition, target.ReceivedKey.KeyChar);
                        target.CursorPosition++;
                    }
                })
                .AddDeleteBackspace()
                .AddEnterToFinish();

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}