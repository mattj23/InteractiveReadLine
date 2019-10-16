using System.Runtime.CompilerServices;

namespace InteractiveReadLine
{
    public static class ExtensionMethods
    {
        public static string ReadLine(this IReadLine provider, HandlerConfig config=null)
        {
            using (provider)
            {
                var handler = new InputHandler(provider, config);
                return handler.ReadLine();
            }
        }

        public static InputHandler Handler(this IReadLine readLine)
        {
            return new InputHandler(readLine);

        }
    }
}