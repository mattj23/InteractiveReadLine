using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Provides a very basic readLine input, with the ability to set the cursor position and write out text
    /// </summary>
    public interface IReadLineProvider
    {

        ConsoleKeyInfo ReadKey();

        void SetText(string text);


        void Start(string prompt = "");

        void Finish();

    }
}