using System;
using System.Collections.Generic;
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine
{
    /// <summary>
    /// The main configuration object for the ReadLineHandler, instances of this class carry all of the objects
    /// which customize and control every aspect of the ReadLineHandler's behavior. Many standard behaviors are
    /// provided through the CommonFormatters/CommonKeyBehaviors/CommonLexers tools, but custom behaviors can
    /// be attached to this object instead.
    /// </summary>
    public class ReadLineConfig
    {
        private ReadLineConfig()
        {
            this.KeyBehaviors = new Dictionary<KeyId, Action<IKeyBehaviorTarget>>();
        }

        /// <summary>
        /// Gets an Action that is invoked with the finalized line when a ReadLine operation completes. The
        /// action is responsible for recording the line in the history. SetUpdatingHistorySource configures
        /// this action automatically, or callers can configure it directly with SetHistoryUpdateAction. It is
        /// null when history updates are disabled, in which case no line is recorded.
        /// </summary>
        public Action<string>? UpdateHistory { get; private set; }

        /// <summary>
        /// Gets the history of entered text, or null when no history source is configured.
        /// </summary>
        public IReadOnlyList<string>? History { get; private set; }

        /// <summary>
        /// Gets the formatter that processes a tokenized line before display, or null when no token-based
        /// formatter is configured. The formatter requires a Lexer.
        /// </summary>
        public Func<TokenizedLine, LineDisplayState>? FormatterFromTokens { get; private set; }

        /// <summary>
        /// Gets the formatter that processes the raw LineState, or null when no line-based formatter is
        /// configured.
        /// </summary>
        public Func<LineState, LineDisplayState>? FormatterFromLine { get; private set; }

        /// <summary>
        /// Gets a dictionary which maps key press information to key behavior methods
        /// </summary>
        public Dictionary<KeyId, Action<IKeyBehaviorTarget>> KeyBehaviors { get; }

        /// <summary>
        /// Gets the default key behavior applied when KeyBehaviors contains no behavior for the pressed key.
        /// The value is null when no default behavior is configured.
        /// </summary>
        public Action<IKeyBehaviorTarget>? DefaultKeyBehavior { get; private set; }
        
        /// <summary>
        /// Gets the lexer that tokenizes a LineState, or null when no lexer is configured. A lexer is required
        /// for auto-completion and certain token-based key behaviors.
        /// </summary>
        public Func<LineState, TokenizedLine>? Lexer { get; private set; }

        /// <summary>
        /// Gets the auto-completion provider, or null when none is configured. The provider receives a
        /// TokenizedLine and returns suggestions for the token under the cursor. It can also modify the
        /// TokenizedLine.
        /// </summary>
        public Func<TokenizedLine, string[]>? AutoCompletion { get; private set; }

        /// <summary>
        /// Gets whether the configuration is capable of auto-completion, which requires both a Lexer
        /// and an auto-completion handler.
        /// </summary>
        public bool CanAutoComplete => this.Lexer != null && this.AutoCompletion != null;

        /// <summary>
        /// Sets the auto-completion handler for the configuration
        /// </summary>
        public ReadLineConfig SetAutoCompletion(Func<TokenizedLine, string[]> handler)
        {
            this.AutoCompletion = handler;
            return this;
        }

        /// <summary>
        /// Sets the lexer for the configuration
        /// </summary>
        public ReadLineConfig SetLexer(Func<LineState, TokenizedLine> lexer)
        {
            this.Lexer = lexer;
            return this;
        }

        /// <summary>
        /// Sets the formatter for the configuration using a formatter based on a tokenized line. This will
        /// require that a lexer be provided for the configuration.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public ReadLineConfig SetFormatter(Func<TokenizedLine, LineDisplayState> formatter)
        {
            this.FormatterFromLine = null;
            this.FormatterFromTokens = formatter;
            return this;
        }

        /// <summary>
        /// Sets the formatter for the configuration using a formatter which only needs a simple LineState.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public ReadLineConfig SetFormatter(Func<LineState, LineDisplayState> formatter)
        {
            this.FormatterFromLine = formatter;
            this.FormatterFromTokens = null;
            return this;
        }

        /// <summary>
        /// Sets the collection of previously entered lines to be used as the command history. Use this setter
        /// if you do not want the history to be automatically updated. Also, make sure to set some key behavior
        /// which will make use of the history.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public ReadLineConfig SetHistorySource(IReadOnlyList<string> history)
        {
            this.History = history;
            return this;
        }

        /// <summary>
        /// Sets the collection of previously entered lines to be used as the command history. This setter will
        /// cause the history list to be automatically updated. Also, make sure to set some key behavior which
        /// will make use of the history.
        /// </summary>
        /// <remarks>
        /// The update action created here skips lines that are empty or contain only whitespace. It also skips
        /// a line that is identical to the most recent entry. These entries therefore do not crowd out useful
        /// history. To record every finalized line, provide your own action through SetHistoryUpdateAction.
        /// </remarks>
        /// <param name="history"></param>
        /// <returns></returns>
        public ReadLineConfig SetUpdatingHistorySource(List<string> history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            this.SetHistoryUpdateAction(text =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                if (history.Count > 0 && history[history.Count - 1] == text)
                    return;

                history.Add(text);
            });

            return this.SetHistorySource(history);
        }

        /// <summary>
        /// Sets the action delegate which will get call when the readline is finalized and the history would otherwise
        /// update itself.  Use this to provide a custom update action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ReadLineConfig SetHistoryUpdateAction(Action<string> action)
        {
            this.UpdateHistory = action;
            return this;
        }

        /// <summary>
        /// Set the default KeyBehavior which is called when no other special action for a key has been registered
        /// in the configuration. Typically this will be used to insert the key character into the input line.
        /// </summary>
        /// <param name="defaultBehavior"></param>
        /// <returns></returns>
        public ReadLineConfig SetDefaultKeyBehavior(Action<IKeyBehaviorTarget> defaultBehavior)
        {
            this.DefaultKeyBehavior = defaultBehavior;
            return this;
        }


        /// <summary>
        /// Gets an empty configuration with absolutely nothing set
        /// </summary>
        public static ReadLineConfig Empty => new ReadLineConfig();

        /// <summary>
        /// Gets a configuration that has the standard key behaviors added to it 
        /// </summary>
        public static ReadLineConfig Basic => new ReadLineConfig().AddStandardKeys();

    }
}
