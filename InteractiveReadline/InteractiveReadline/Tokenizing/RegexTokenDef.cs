using System.Text.RegularExpressions;

namespace InteractiveReadLine.Tokenizing
{
    public class RegexTokenDef
    {
        public RegexTokenDef(string pattern, int typeCode=0)
        {
            this.Pattern = pattern;
            this.TypeCode = typeCode;

            if (!this.Pattern.StartsWith("^"))
                this.Pattern = "^" + this.Pattern;

            this.Regex = new Regex(this.Pattern);
        }

        public string Pattern { get; }

        public int TypeCode { get; }

        public Regex Regex { get; }
    }
}