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

        public bool IsTesting { get; set; }

        public Func<LineState, TokenizedLine> Tokenizer { get; private set; }

        public Func<TokenizedLine, string[]> AutoCompletion { get; private set; }

        public bool CanAutoComplete => this.Tokenizer != null && this.AutoCompletion != null;

        public ReadLineConfig SetAutoCompletion(Func<TokenizedLine, string[]> handler)
        {
            this.AutoCompletion = handler;
            return this;
        }

        public ReadLineConfig SetTokenizer(Func<LineState, TokenizedLine> tokenizer)
        {
            this.Tokenizer = tokenizer;
            return this;
        }

        public Dictionary<KeyId, Action<IKeyBehaviorTarget>> KeyBehaviors { get; }

        public static ReadLineConfig Test() => new ReadLineConfig() {IsTesting = true};

        public static ReadLineConfig Empty() => new ReadLineConfig();

    }
}