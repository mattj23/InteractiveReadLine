using System;
using InteractiveReadLine.Components;

namespace InteractiveReadLine.Behaviors
{
    public static class BehaviorExtensionMethods
    {
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, KeyId key, Action<IKeyBehaviorTarget> action)
        {
            config.KeyBehaviors.Add(key, action);
            return config;
        }

        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, char key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key), action);
        }

        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, false, false, false), action);
        }

        public static ReadLineConfig AddCtrlKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, true, false, false), action);
        }

        public static ReadLineConfig AddDeleteBackspace(this ReadLineConfig config)
        {
            return config.AddKeyBehavior(ConsoleKey.Delete, StandardBehaviors.Delete)
                .AddKeyBehavior(ConsoleKey.Backspace, StandardBehaviors.Backspace);
        }

        public static ReadLineConfig AddStandardKeys(this ReadLineConfig config)
        {
            return config.AddDeleteBackspace()
                .AddKeyBehavior(ConsoleKey.LeftArrow, StandardBehaviors.LeftArrow)
                .AddKeyBehavior(ConsoleKey.RightArrow, StandardBehaviors.RightArrow);
        }

    }
}