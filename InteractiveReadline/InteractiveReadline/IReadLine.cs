using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Provides a very basic ReadLine input, with the ability to start and finish an input instance
    /// </summary>
    public interface IReadLine : IDisposable
    {

        ConsoleKeyInfo ReadKey();

        void SetInputText(string text, int cursor);


    }
}