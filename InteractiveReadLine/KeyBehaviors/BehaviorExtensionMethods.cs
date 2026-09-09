using System;

namespace InteractiveReadLine.KeyBehaviors
{
    public static class BehaviorExtensionMethods
    {
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, KeyId key, Action<IKeyBehaviorTarget> action)
        {
            // Use assignment so that a binding registered later replaces an earlier binding without throwing.
            // This lets callers start with a pre-built configuration and override individual keys.
            config.KeyBehaviors[key] = action;
            return config;
        }

        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            bool control, bool alt, bool shift, Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, control, alt, shift), action);
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
            return config.AddKeyBehavior(ConsoleKey.Delete, CommonKeyBehaviors.Delete)
                .AddKeyBehavior(ConsoleKey.Backspace, CommonKeyBehaviors.Backspace);
        }

        public static ReadLineConfig AddEnterToFinish(this ReadLineConfig config)
        {
            return config.AddKeyBehavior(ConsoleKey.Enter, CommonKeyBehaviors.Finish);
        }

        /// <summary>
        /// Binds Ctrl+C to abandon the current line, matching conventional shell behavior.
        /// </summary>
        /// <remarks>
        /// The ConsoleReadLine provider puts the console into a mode where Ctrl+C arrives as an ordinary
        /// keypress rather than terminating the process, and restores the previous mode when it is disposed.
        /// Without this binding, Ctrl+C has no effect while a line is being read.
        /// </remarks>
        public static ReadLineConfig AddCancelKeys(this ReadLineConfig config)
        {
            return config.AddCtrlKeyBehavior(ConsoleKey.C, CommonKeyBehaviors.Cancel);
        }

        public static ReadLineConfig AddHomeAndEndKeys(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Home, CommonKeyBehaviors.MoveCursorToStart)
                .AddKeyBehavior(ConsoleKey.End, CommonKeyBehaviors.MoveCursorToEnd);
        }

        public static ReadLineConfig AddArrowMovesCursor(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.LeftArrow, CommonKeyBehaviors.MoveCursorLeft)
                .AddKeyBehavior(ConsoleKey.RightArrow, CommonKeyBehaviors.MoveCursorRight);
        }
        public static ReadLineConfig AddUpDownHistoryNavigation(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonKeyBehaviors.HistoryPrevious)
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonKeyBehaviors.HistoryNext);
        }

        public static ReadLineConfig AddCtrlNavKeys(this ReadLineConfig config)
        {
            return config
                .AddCtrlKeyBehavior(ConsoleKey.A, CommonKeyBehaviors.MoveCursorToStart)
                .AddCtrlKeyBehavior(ConsoleKey.B, CommonKeyBehaviors.MoveCursorLeft)
                .AddCtrlKeyBehavior(ConsoleKey.E, CommonKeyBehaviors.MoveCursorToEnd)
                .AddCtrlKeyBehavior(ConsoleKey.F, CommonKeyBehaviors.MoveCursorRight)
                .AddCtrlKeyBehavior(ConsoleKey.H, CommonKeyBehaviors.Backspace)
                .AddCtrlKeyBehavior(ConsoleKey.K, CommonKeyBehaviors.CutToEnd)
                .AddCtrlKeyBehavior(ConsoleKey.U, CommonKeyBehaviors.CutToStart)
                .AddCtrlKeyBehavior(ConsoleKey.L, CommonKeyBehaviors.ClearAll)
                .AddCtrlKeyBehavior(ConsoleKey.M, CommonKeyBehaviors.Finish)
                .AddCtrlKeyBehavior(ConsoleKey.N, CommonKeyBehaviors.HistoryNext)
                .AddCtrlKeyBehavior(ConsoleKey.P, CommonKeyBehaviors.HistoryPrevious)
                .AddCtrlKeyBehavior(ConsoleKey.D, CommonKeyBehaviors.DeleteOrEndOfInput)
                .AddCtrlKeyBehavior(ConsoleKey.W, CommonKeyBehaviors.CutPreviousWord)
                // GNU Readline and many terminal applications conventionally use Ctrl+Y for pasting. Ctrl+V
                // conventionally inserts the next character literally, and many terminals intercept it.
                .AddCtrlKeyBehavior(ConsoleKey.Y, CommonKeyBehaviors.Paste);
        }

        /// <summary>
        /// Adds a set of standard keys to the configuration, including the default of inserting printable
        /// characters, enter to finish the line, delete, backspace, the left and right arrow keys, and
        /// Ctrl+C to abandon the line.
        /// </summary>
        public static ReadLineConfig AddStandardKeys(this ReadLineConfig config)
        {
            return config
                .SetDefaultKeyBehavior(CommonKeyBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddDeleteBackspace()
                .AddHomeAndEndKeys()
                .AddUpDownHistoryNavigation()
                .AddArrowMovesCursor()
                .AddCancelKeys();
        }

        public static ReadLineConfig AddTabAutoComplete(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Tab, CommonKeyBehaviors.AutoCompleteNext)
                .AddKeyBehavior(new KeyId(ConsoleKey.Tab, false, false, true), CommonKeyBehaviors.AutoCompletePrevious);
        }
    }
}
