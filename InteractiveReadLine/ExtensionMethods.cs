using System.Threading;
using System.Threading.Tasks;

namespace InteractiveReadLine
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Interactively read a line of text from the provider, using a specified configuration if one is
        /// given as an optional argument.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        /// <param name="cancellationToken">A token that cancels the read.</param>
        /// <returns>
        /// The text read from the user; null if the user signaled the end of input; or an empty string if the
        /// user abandoned the line. Use Read to distinguish an abandoned line from an entered empty line.
        /// </returns>
        /// <exception cref="System.OperationCanceledException">
        /// The token was canceled before input completed.
        /// </exception>
        public static string? ReadLine(this IReadLineProvider provider, ReadLineConfig? config=null,
            CancellationToken cancellationToken=default)
        {
            return provider.Read(config, cancellationToken).ToText();
        }

        /// <summary>
        /// Interactively reads a line of text from the provider and returns a result that describes the text
        /// and how the user ended the interaction. The method disposes the provider before returning.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        /// <param name="cancellationToken">
        /// A token that cancels the read. The provider is still disposed if the read is canceled, so the
        /// console is left in the state it would have been after an ordinary read.
        /// </param>
        /// <returns>The completed interaction result.</returns>
        /// <exception cref="System.OperationCanceledException">
        /// The token was canceled before input completed.
        /// </exception>
        public static ReadLineResult Read(this IReadLineProvider provider, ReadLineConfig? config=null,
            CancellationToken cancellationToken=default)
        {
            using (provider)
            {
                var handler = new ReadLineHandler(provider, config);
                return handler.Read(cancellationToken);
            }
        }

        /// <summary>
        /// Interactively reads a line of text from the provider without blocking a thread while waiting for
        /// keys. The method disposes the provider before returning.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        /// <param name="cancellationToken">A token that cancels the read.</param>
        /// <returns>
        /// The text read from the user; null if the user signaled the end of input; or an empty string if the
        /// user abandoned the line. Use ReadAsync to distinguish an abandoned line from an entered empty line.
        /// </returns>
        /// <exception cref="System.OperationCanceledException">
        /// The token was canceled before input completed.
        /// </exception>
        public static async Task<string?> ReadLineAsync(this IReadLineProvider provider, ReadLineConfig? config=null,
            CancellationToken cancellationToken=default)
        {
            var result = await provider.ReadAsync(config, cancellationToken).ConfigureAwait(false);
            return result.ToText();
        }

        /// <summary>
        /// Interactively reads a line of text from the provider without blocking a thread while waiting for
        /// keys, returning a result that describes the text and how the user ended the interaction. The method
        /// disposes the provider before returning.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        /// <param name="cancellationToken">
        /// A token that cancels the read. The provider is still disposed if the read is canceled, so the
        /// console is left in the state it would have been after an ordinary read.
        /// </param>
        /// <returns>The completed interaction result.</returns>
        /// <exception cref="System.OperationCanceledException">
        /// The token was canceled before input completed.
        /// </exception>
        public static async Task<ReadLineResult> ReadAsync(this IReadLineProvider provider, ReadLineConfig? config=null,
            CancellationToken cancellationToken=default)
        {
            using (provider)
            {
                var handler = new ReadLineHandler(provider, config);
                return await handler.ReadAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
