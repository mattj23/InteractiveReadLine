using LineFormatter = System.Func<InteractiveReadLine.LineState, InteractiveReadLine.LineDisplayState>;
using TokenFormatter = System.Func<InteractiveReadLine.Tokenizing.TokenizedLine, InteractiveReadLine.LineDisplayState>;

namespace InteractiveReadLine.Formatting
{
    public class CommonFormatters
    {
        public static LineFormatter FixedPrompt(string prompt)
        {
            return state => new LineDisplayState(prompt, state.Text, string.Empty, state.Cursor);
        }
    }
}