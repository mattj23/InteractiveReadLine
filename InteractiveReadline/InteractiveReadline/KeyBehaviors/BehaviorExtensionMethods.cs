using System;

namespace InteractiveReadLine.KeyBehaviors
{
    public static class BehaviorExtensionMethods
    {
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, KeyId key, Action<IKeyBehaviorTarget> action)
        {
            config.KeyBehaviors.Add(key, action);
            return config;
        }

        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            bool control, bool alt, bool shift, Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, false, false, false), action);
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
            return config.AddKeyBehavior(ConsoleKey.Delete, CommonBehaviors.Delete)
                .AddKeyBehavior(ConsoleKey.Backspace, CommonBehaviors.Backspace);
        }

        public static ReadLineConfig AddEnterToFinish(this ReadLineConfig config)
        {
            return config.AddKeyBehavior(ConsoleKey.Enter, CommonBehaviors.Finish);
        }

        public static ReadLineConfig AddHomeAndEndKeys(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Home, CommonBehaviors.MoveCursorToStart)
                .AddKeyBehavior(ConsoleKey.End, CommonBehaviors.MoveCursorToEnd);
        }

        public static ReadLineConfig AddArrowMovesCursor(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.LeftArrow, CommonBehaviors.MoveCursorLeft)
                .AddKeyBehavior(ConsoleKey.RightArrow, CommonBehaviors.MoveCursorRight);
        }
        public static ReadLineConfig AddUpDownHistoryNavigation(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonBehaviors.HistoryPrevious)
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonBehaviors.HistoryNext);
        }

        public static ReadLineConfig AddCtrlNavKeys(this ReadLineConfig config)
        {
            return config
                .AddCtrlKeyBehavior(ConsoleKey.A, CommonBehaviors.MoveCursorToStart)
                .AddCtrlKeyBehavior(ConsoleKey.B, CommonBehaviors.MoveCursorLeft)
                .AddCtrlKeyBehavior(ConsoleKey.E, CommonBehaviors.MoveCursorToEnd)
                .AddCtrlKeyBehavior(ConsoleKey.F, CommonBehaviors.MoveCursorRight)
                .AddCtrlKeyBehavior(ConsoleKey.H, CommonBehaviors.Backspace)
                .AddCtrlKeyBehavior(ConsoleKey.L, CommonBehaviors.Finish)
                .AddCtrlKeyBehavior(ConsoleKey.K, CommonBehaviors.CutToEnd)
                .AddCtrlKeyBehavior(ConsoleKey.U, CommonBehaviors.CutToStart)
                .AddCtrlKeyBehavior(ConsoleKey.L, CommonBehaviors.ClearAll)
                .AddCtrlKeyBehavior(ConsoleKey.M, CommonBehaviors.Finish)
                .AddCtrlKeyBehavior(ConsoleKey.N, CommonBehaviors.HistoryNext)
                .AddCtrlKeyBehavior(ConsoleKey.P, CommonBehaviors.HistoryPrevious)
                .AddCtrlKeyBehavior(ConsoleKey.D, CommonBehaviors.Delete)
                .AddCtrlKeyBehavior(ConsoleKey.W, CommonBehaviors.CutPreviousWord);
        }

        /// <summary>
        /// Adds a set of standard keys to the configuration, including the default of inserting printable
        /// characters, enter to finish the line, delete, backspace, and the left and right arrow keys.
        /// </summary>
        public static ReadLineConfig AddStandardKeys(this ReadLineConfig config)
        {
            return config
                .SetDefaultKeyBehavior(CommonBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddDeleteBackspace()
                .AddHomeAndEndKeys()
                .AddUpDownHistoryNavigation()
                .AddArrowMovesCursor();
        }

        public static ReadLineConfig AddTabAutoComplete(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Tab, CommonBehaviors.AutoCompleteNext)
                .AddKeyBehavior(new KeyId(ConsoleKey.Tab, false, false, true), CommonBehaviors.AutoCompletePrevious);
        }
    }
}