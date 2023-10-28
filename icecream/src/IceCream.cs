using System;
using System.Runtime.CompilerServices;
using System.Text;
using static icecream.IceCreamCore;

namespace icecream
{
    public static class IceCream
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

        public static string IceFormat<T>(this T value, string label = null, [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            return IceFormatInternal(value, label, memberName, lineNumber, filePath);
        }

        public static T ic<T>(this T value, string label = null, [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            return IcInternal(value, label, memberName, lineNumber, filePath);
        }

        public static void SetIncludeContext(bool includeContext)
        {
            _settings.IncludeContext = includeContext;
        }

        public static void SetPrefix(string prefix)
        {
            _settings.Prefix = prefix;
        }

        public static void SetUseAbsPath(bool useAbsPath)
        {
            _settings.UseAbsPath = useAbsPath;
        }

        public static void SetOutputAction(Action<string> outputAction)
        {
            _settings.OutputAction = outputAction;
        }

        public static void SetArgToStringFunction(Func<object, string> argToStringFunction)
        {
            _settings.ArgToStringFunction = argToStringFunction;
        }

        public static void SetLabelColor(ConsoleColor? labelColor)
        {
            _settings.LabelColor = labelColor;
        }

        public static void SetFieldColor(ConsoleColor? fieldColor)
        {
            _settings.FieldColor = fieldColor;
        }

        public static void SetValueColor(ConsoleColor? valueColor)
        {
            _settings.ValueColor = valueColor;
        }

        public static void SetEncoding(Encoding encoding)
        {
            _settings.Encoding = encoding;
        }
    }
}