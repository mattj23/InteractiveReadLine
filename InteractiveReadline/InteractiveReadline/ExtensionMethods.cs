using System.Runtime.CompilerServices;

namespace InteractiveReadLine
{
    public static class ExtensionMethods
    {
        public static InputHandler Handler(this IReadLine readLine)
        {
            return new InputHandler(readLine);
        }
    }
}