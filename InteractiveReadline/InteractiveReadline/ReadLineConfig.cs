using System;
using System.Collections.Generic;
using System.Text;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine
{
    public class ReadLineConfig
    {

        private ReadLineConfig()
        {
            this.KeyBehaviors = new Dictionary<KeyId, Action<IKeyBehaviorTarget>>();
        }

        /// <summary>
        /// Gets a dictionary which maps key press information to key behavior methods
        /// </summary>
        public Dictionary<KeyId, Action<IKeyBehaviorTarget>> KeyBehaviors { get; }
        
        /// <summary>
        /// Gets the lexer for the readline handler to use, which tokenizes a LineState object.
        /// A non-null lexer is a critical component of autocompletion and certain token-based key behaviors 
        /// </summary>
        public Func<LineState, TokenizedLine> Lexer { get; private set; }

        /// <summary>
        /// Gets the autocompletion provider, which is a method that takes a TokenizedLine object
        /// and returns a list of suggestions for the token under the cursor. The TokenizedLine can
        /// also be modified by the autocompletion method.
        /// </summary>
        public Func<TokenizedLine, string[]> AutoCompletion { get; private set; }

        /// <summary>
        /// Gets whether the configuration is capable of autocompletion, which requires both a Lexer
        /// and an autocompletion handler.
        /// </summary>
        public bool CanAutoComplete => this.Lexer != null && this.AutoCompletion != null;

        /// <summary>
        /// Sets the autocompletion handler for the configuration
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


        public static ReadLineConfig Empty() => new ReadLineConfig();

    }
}