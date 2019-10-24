using System;
using System.Security.Cryptography;
using System.Text;
using LineFormatter = System.Func<InteractiveReadLine.LineState, InteractiveReadLine.LineDisplayState>;
using TokenFormatter = System.Func<InteractiveReadLine.Tokenizing.TokenizedLine, InteractiveReadLine.LineDisplayState>;

namespace InteractiveReadLine.Formatting
{
    public static class CommonFormatters
    {
        /// <summary>
        /// Provides a formatter that displays a fixed prompt in front of the input line, or wraps another
        /// formatter overwriting its prefix with the supplied prompt
        /// </summary>
        /// <param name="prompt">the prompt text to display</param>
        /// <param name="formatter"></param>
        public static LineFormatter FixedPrompt(FormattedText prompt, LineFormatter formatter = null)
        {
            return state =>
            {
                if (formatter == null)
                    return new LineDisplayState(prompt, state.Text, string.Empty, state.Cursor);
                var result = formatter.Invoke(state);
                return new LineDisplayState(prompt, result.LineBody, result.Suffix, result.Cursor);
            };
        }

        /// <summary>
        /// A formatter that blanks out the entered characters, and has no prefix or suffix
        /// </summary>
        public static LineDisplayState PasswordBlank(LineState state) => new LineDisplayState(string.Empty, string.Empty, string.Empty, 0);

        /// <summary>
        /// A formatter that puts out a variable length bar based on the first byte in a SHA256 hash
        /// of the entered password. Provides visual feedback without revealing anything about the
        /// password itself
        /// </summary>
        public static LineDisplayState PasswordBar(LineState state)
        {
            var hash = SHA256.Create();
            var result = hash.ComputeHash(Encoding.ASCII.GetBytes(state.Text));
            var l1 = (int) Math.Round(20.0 * ((float) result[0]) / 255.0);
            var l2 = (int) Math.Round(20.0 * ((float) result[10]) / 255.0);
            var builder = new StringBuilder("[");
            for (int i = 0; i < 20; i++)
            {
                if ((i < l1 && i < l2) || (i > l1 && i > l2))
                    builder.Append(' ');
                else
                    builder.Append('=');
            }

            builder.Append(']');

            return new LineDisplayState(string.Empty, builder.ToString(), string.Empty, builder.Length);
        }

        public static LineFormatter Password => PasswordBar;

        /// <summary>
        /// A formatter that converts the password characters to stars
        /// </summary>
        public static LineDisplayState PasswordStars(LineState state)
        {
            return new LineDisplayState(string.Empty, new string('*', state.Text.Length), string.Empty, state.Cursor);
        }


        /// <summary>
        /// Adds a fixed prompt to the given formatter, overwriting the prefix
        /// </summary>
        public static LineFormatter FixedPrompt(this LineFormatter formatter, FormattedText prompt) => FixedPrompt(prompt, formatter);
    }
}