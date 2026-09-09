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
        /// <returns>
        /// The text read from the user; null if the user signaled the end of input; or an empty string if the
        /// user abandoned the line. Use Read to distinguish an abandoned line from an entered empty line.
        /// </returns>
        public static string ReadLine(this IReadLineProvider provider, ReadLineConfig config=null)
        {
            return provider.Read(config).ToText();
        }

        /// <summary>
        /// Interactively reads a line of text from the provider and returns a result that describes the text
        /// and how the user ended the interaction. The method disposes the provider before returning.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        public static ReadLineResult Read(this IReadLineProvider provider, ReadLineConfig config=null)
        {
            using (provider)
            {
                var handler = new ReadLineHandler(provider, config);
                return handler.Read();
            }
        }

    }
}
