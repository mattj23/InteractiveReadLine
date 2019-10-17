using System.Text;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// Provides the unified target of key behaviors, exposing a standard amount of state which the
    /// behavior method can act upon
    /// </summary>
    public interface IKeyBehaviorTarget
    {
        StringBuilder TextBuffer { get; }

        int CursorPosition { get; set; }

        void AutoCompleteNext();

        void AutoCompletePrevious();
    }
}