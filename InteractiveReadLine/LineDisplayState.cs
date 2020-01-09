using InteractiveReadLine.Formatting;

namespace InteractiveReadLine
{
    /// <summary>
    /// Holds all of the information to present to the user through the view.  The LineDisplayState is the output
    /// of a formatter, and will be written directly to the provider.
    /// </summary>
    public class LineDisplayState
    {
        public LineDisplayState(FormattedText prefix, FormattedText lineBody, FormattedText suffix, int cursor)
        {
            Prefix = prefix;
            LineBody = lineBody;
            Suffix = suffix;
            Cursor = cursor;
        }

        /// <summary>
        /// Gets the prefix, which is displayed before the line body
        /// </summary>
        public FormattedText Prefix { get; }

        /// <summary>
        /// Gets the line's body text, which is typically the text entered by the user or some representation of
        /// what they've entered
        /// </summary>
        public FormattedText LineBody { get; }

        /// <summary>
        /// Gets the position of the cursor as an integer offset from the beginning of the LineBody
        /// </summary>
        public int Cursor { get; }

        /// <summary>
        /// Gets the suffix, which is displayed after the line body
        /// </summary>
        public FormattedText Suffix { get; }
    }
}