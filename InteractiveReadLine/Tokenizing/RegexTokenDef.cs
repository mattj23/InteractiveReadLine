using System.Text.RegularExpressions;

namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// Defines a token type for a regular-expression parser. The definition contains a matching pattern,
    /// automatically anchored with ^ when necessary, and an optional type code for matching tokens.
    /// </summary>
    public class RegexTokenDef
    {
        /// <summary>
        /// Creates a token definition from a regular expression pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern that identifies the token. The constructor adds a leading ^ when the pattern does not
        /// already have one.
        /// </param>
        /// <param name="typeCode">A code that identifies this token type in the resulting tokens.</param>
        public RegexTokenDef(string pattern, int typeCode=0)
        {
            this.Pattern = pattern;
            this.TypeCode = typeCode;

            if (!this.Pattern.StartsWith("^"))
                this.Pattern = "^" + this.Pattern;

            this.Regex = new Regex(this.Pattern);
        }

        /// <summary>
        /// Gets the string version of the regex pattern defined to match tokens of this type
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets the optional type code to assign to tokens matched by the Pattern property
        /// </summary>
        public int TypeCode { get; }

        /// <summary>
        /// Gets the regular expression created from the Pattern property.
        /// </summary>
        public Regex Regex { get; }
    }
}
