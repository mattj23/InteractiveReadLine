using System;
using System.Threading;
using System.Threading.Tasks;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine
{
    /// <summary>
    /// The underlying contract for any object which is to provide ReadLine functionality to the input handler.
    /// </summary>
    /// <remarks>
    /// Any object which is to provide a functioning backend on which the ReadLineHandler can act must implement these
    /// features: returning a single ConsoleKeyInfo when asked, both synchronously and asynchronously, writing out
    /// text and a cursor position on the currently active line (clearing whatever was there before), and inserting
    /// text to some output without interfering with the active display line.
    ///
    /// Finally, the provider must implement IDisposable; as it will be disposed by the handler when the input of
    /// a line has been completed. The provider must then clean up the view in some fashion (on a standard System.Console
    /// provider this involves writing a newline to the console)
    /// </remarks>
    public interface IReadLineProvider : IDisposable
    {
        /// <summary>
        /// Waits synchronously for a ConsoleKeyInfo to be returned from some underlying input source.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token that cancels the wait. An implementation whose underlying wait cannot be interrupted
        /// should poll for input becoming available rather than blocking, so that the token can be observed.
        /// </param>
        /// <returns>Information about the key that was read.</returns>
        /// <exception cref="OperationCanceledException">The token was canceled before a key arrived.</exception>
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken = default);

        /// <summary>
        /// Waits asynchronously for a ConsoleKeyInfo to be returned from some underlying input source.
        /// </summary>
        /// <remarks>
        /// A backend whose input arrives through an event or an already asynchronous API should implement this
        /// natively. A backend whose ReadKey does not block, such as one reading from an already populated
        /// queue, may return Task.FromResult(ReadKey(cancellationToken)). Avoid wrapping a blocking read in
        /// Task.Run because it occupies a thread until the user presses a key, and the token cannot interrupt it.
        /// </remarks>
        /// <param name="cancellationToken">A token that cancels the wait.</param>
        /// <returns>A task producing information about the key that was read.</returns>
        /// <exception cref="OperationCanceledException">The token was canceled before a key arrived.</exception>
        Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Displays the LineDisplayState to the visual output, including the cursor position
        /// </summary>
        /// <param name="state">The line display state (prefix, suffix, text, and cursor position) to be displayed</param>
        void SetDisplay(LineDisplayState state);
        
        /// <summary>
        /// Inserts text to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row. This is used to interrupt the user with a message or
        /// information while the ReadLine is still active
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        void InsertText(FormattedText text);

    }
}
