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

        public Func<Tokenize, Tokens> Tokenizer { get; private set; }

        public ReadLineConfig SetTokenizer(Func<Tokenize, Tokens> tokenizer)
        {
            this.Tokenizer = tokenizer;
            return this;
        }

        public Dictionary<KeyId, Action<IKeyBehaviorTarget>> KeyBehaviors { get; }

        public static ReadLineConfig Test() => new ReadLineConfig() {IsTesting = true};

        public static ReadLineConfig Empty() => new ReadLineConfig();

    }
}