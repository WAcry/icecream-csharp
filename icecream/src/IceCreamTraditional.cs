using System.Runtime.CompilerServices;
using static icecream.IceCreamCore;

namespace icecream
{
    public static class IceCreamTraditional
    {
        public static void Configure(IceCreamSettings settings = null)
        {
            _settings = settings ?? new IceCreamSettings();
        }

        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        public static string IceFormat<T>(T value, string label = null, [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            return IceFormatInternal(value, label, memberName, lineNumber, filePath);
        }

        public static T ic<T>(T value, string label = null, [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            return IcInternal(value, label, memberName, lineNumber, filePath);
        }
    }
}