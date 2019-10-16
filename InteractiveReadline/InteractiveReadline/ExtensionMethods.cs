using System.Runtime.CompilerServices;

namespace InteractiveReadLine
{
    public static class ExtensionMethods
    {
        public static string ReadLine(this IReadLine provider, ReadLineConfig config=null)
        {
            using (provider)
            {
                var handler = new InputHandler(provider, config);
                return handler.ReadLine();
            }
        }

    }
}