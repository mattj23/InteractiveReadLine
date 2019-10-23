using InteractiveReadLine.Formatting;

namespace InteractiveReadLine
{
    public class LineDisplayState
    {
        public LineDisplayState(FormattedText prefix, FormattedText lineBody, FormattedText suffix, int cursor)
        {
            Prefix = prefix;
            LineBody = lineBody;
            Suffix = suffix;
            Cursor = cursor;
        }

        public FormattedText Prefix { get; }

        public FormattedText LineBody { get; }

        public int Cursor { get; }

        public FormattedText Suffix { get; }
    }
}