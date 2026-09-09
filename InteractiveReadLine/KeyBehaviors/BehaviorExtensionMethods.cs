using System;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// Provides extension methods that register key behaviors on a configuration. Each method returns the
    /// supplied configuration for chaining and replaces any existing binding for the same key.
    /// </summary>
    public static class BehaviorExtensionMethods
    {
        /// <summary>
        /// Binds a behavior to a key identified by a KeyId that contains the key and its modifiers.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <param name="key">The key which invokes the behavior</param>
        /// <param name="action">The behavior to run when the key is pressed</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, KeyId key, Action<IKeyBehaviorTarget> action)
        {
            // Use assignment so that a binding registered later replaces an earlier binding without throwing.
            // This lets callers start with a pre-built configuration and override individual keys.
            config.KeyBehaviors[key] = action;
            return config;
        }

        /// <summary>
        /// Binds a behavior to a key pressed together with the given modifiers.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <param name="key">The key which invokes the behavior</param>
        /// <param name="control">Whether the control key must be held</param>
        /// <param name="alt">Whether the alt key must be held</param>
        /// <param name="shift">Whether the shift key must be held</param>
        /// <param name="action">The behavior to run when the key is pressed</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            bool control, bool alt, bool shift, Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, control, alt, shift), action);
        }

        /// <summary>
        /// Binds a behavior to a character, whichever combination of keys produced it. A character binding is
        /// matched before the key and modifier bindings are considered.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <param name="key">The character which invokes the behavior</param>
        /// <param name="action">The behavior to run when the character is typed</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, char key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key), action);
        }

        /// <summary>
        /// Binds a behavior to a key pressed with no modifiers.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <param name="key">The key which invokes the behavior</param>
        /// <param name="action">The behavior to run when the key is pressed</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, false, false, false), action);
        }

        /// <summary>
        /// Binds a behavior to a key pressed together with the control key.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <param name="key">The key which invokes the behavior when control is held</param>
        /// <param name="action">The behavior to run when the key is pressed</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddCtrlKeyBehavior(this ReadLineConfig config, ConsoleKey key,
            Action<IKeyBehaviorTarget> action)
        {
            return config.AddKeyBehavior(new KeyId(key, true, false, false), action);
        }

        /// <summary>
        /// Binds the delete key to removing the character under the cursor, and the backspace key to removing
        /// the character before it.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddDeleteBackspace(this ReadLineConfig config)
        {
            return config.AddKeyBehavior(ConsoleKey.Delete, CommonKeyBehaviors.Delete)
                .AddKeyBehavior(ConsoleKey.Backspace, CommonKeyBehaviors.Backspace);
        }

        /// <summary>
        /// Binds the Enter key to finish the line and return from the read line operation.
        /// </summary>
        /// <param name="config">The configuration to add the binding to</param>
        /// <returns>The same configuration, for chaining</returns>
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

        /// <summary>
        /// Binds the home and end keys to moving the cursor to the start and end of the line.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddHomeAndEndKeys(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Home, CommonKeyBehaviors.MoveCursorToStart)
                .AddKeyBehavior(ConsoleKey.End, CommonKeyBehaviors.MoveCursorToEnd);
        }

        /// <summary>
        /// Binds the left and right arrow keys to moving the cursor one character in their direction.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddArrowMovesCursor(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.LeftArrow, CommonKeyBehaviors.MoveCursorLeft)
                .AddKeyBehavior(ConsoleKey.RightArrow, CommonKeyBehaviors.MoveCursorRight);
        }
        /// <summary>
        /// Binds the up and down arrow keys to step backward and forward through history. The
        /// bindings do nothing unless the configuration also has a history source.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddUpDownHistoryNavigation(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.UpArrow, CommonKeyBehaviors.HistoryPrevious)
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonKeyBehaviors.HistoryNext);
        }

        /// <summary>
        /// Binds the control key navigation and editing commands familiar from Bash: cursor movement with
        /// Ctrl+A, B, E and F, backspace with Ctrl+H, cutting with Ctrl+K, U and W, pasting with Ctrl+Y,
        /// clearing with Ctrl+L, finishing with Ctrl+M, history with Ctrl+N and P, and Ctrl+D to delete
        /// forward or signal the end of input on an empty line.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
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

        /// <summary>
        /// Binds Tab to cycle forward through auto-complete suggestions and Shift+Tab to cycle backward. The
        /// bindings do nothing unless the configuration also has a lexer and a suggestion
        /// provider.
        /// </summary>
        /// <param name="config">The configuration to add the bindings to</param>
        /// <returns>The same configuration, for chaining</returns>
        public static ReadLineConfig AddTabAutoComplete(this ReadLineConfig config)
        {
            return config
                .AddKeyBehavior(ConsoleKey.Tab, CommonKeyBehaviors.AutoCompleteNext)
                .AddKeyBehavior(new KeyId(ConsoleKey.Tab, false, false, true), CommonKeyBehaviors.AutoCompletePrevious);
        }
    }
}
