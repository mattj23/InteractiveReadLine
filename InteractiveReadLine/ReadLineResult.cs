using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Describes how a read line operation came to an end.
    /// </summary>
    public enum ReadLineResultKind
    {
        /// <summary>
        /// The user finished a line of input, typically by pressing enter. The text is available on the result.
        /// </summary>
        Line,

        /// <summary>
        /// The user abandoned the line, typically by pressing Ctrl+C. Whatever had been typed is discarded.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The user signaled that there is no more input, typically by pressing Ctrl+D on an empty line.
        /// </summary>
        EndOfInput
    }

    /// <summary>
        /// Represents the outcome of a read line operation: a completed line or one of two outcomes that do not
        /// produce a line.
    /// </summary>
    /// <remarks>
    /// The simpler ReadLine methods flatten this to a string, returning null for EndOfInput and an empty
    /// string for Cancelled. Use the Read methods and this type when the difference matters, for example to
    /// tell an abandoned line apart from a genuinely empty one.
    /// </remarks>
    public readonly struct ReadLineResult : IEquatable<ReadLineResult>
    {
        private ReadLineResult(ReadLineResultKind kind, string? text)
        {
            this.Kind = kind;
            this.Text = text;
        }

        /// <summary>
        /// Gets how the read line operation ended.
        /// </summary>
        public ReadLineResultKind Kind { get; }

        /// <summary>
        /// Gets the text that the user entered. This value is non-null when Kind is Line and null otherwise.
        /// </summary>
        public string? Text { get; }

        /// <summary>
        /// Gets whether the operation produced a line of text.
        /// </summary>
        public bool IsLine => this.Kind == ReadLineResultKind.Line;

        /// <summary>
        /// Creates a result carrying a finished line of text.
        /// </summary>
        public static ReadLineResult ForLine(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return new ReadLineResult(ReadLineResultKind.Line, text);
        }

        /// <summary>
        /// Gets a result representing a line the user abandoned.
        /// </summary>
        public static ReadLineResult Cancelled => new ReadLineResult(ReadLineResultKind.Cancelled, null);

        /// <summary>
        /// Gets a result representing the end of the user's input.
        /// </summary>
        public static ReadLineResult EndOfInput => new ReadLineResult(ReadLineResultKind.EndOfInput, null);

        /// <summary>
        /// Flattens this result to a string, returning the text for a finished line, null for the end of
        /// input, and an empty string for a line the user abandoned.
        /// </summary>
        public string? ToText()
        {
            switch (this.Kind)
            {
                case ReadLineResultKind.Line:
                    return this.Text;
                case ReadLineResultKind.Cancelled:
                    return string.Empty;
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public bool Equals(ReadLineResult other) => this.Kind == other.Kind && this.Text == other.Text;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ReadLineResult other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) this.Kind * 397) ^ (this.Text?.GetHashCode() ?? 0);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            this.IsLine ? $"{this.Kind}: \"{this.Text}\"" : this.Kind.ToString();
    }
}
