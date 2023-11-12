using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static icecream.IceCreamCore;

namespace icecream
{
    public static class IceCream
    {
        /// <summary>
        /// This function is like .ic() but the output is returned as a string instead of being printed to the console.
        /// Call this function after the value you want to print. (e.g. "Hello".IceFormat())
        /// A label parameter can be passed to label the output. (e.g. "Hello".IceFormat("greeting"))
        /// </summary>
        public static string IceFormat<T>(
            this T value,
            object label = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = ""
#if NETFRAMEWORK || NETSTANDARD
#else
            , [CallerArgumentExpression("value")] string arg = null
#endif
        )
        {
            return IceFormatInternal(value, label?.ToString(), memberName, lineNumber, filePath
#if NETFRAMEWORK || NETSTANDARD
#else
                , arg
#endif
            );
        }

        /// <summary>
        /// This function prints the value with context to the console and returns the original value.
        /// Call this function after the value you want to print. (e.g. "Hello".ic())
        /// A label parameter can be passed to label the output. (e.g. "Hello".ic("greeting"))
        /// </summary>
        public static T ic<T>(
            this T value,
            object label = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = ""
#if NETFRAMEWORK || NETSTANDARD
#else
            , [CallerArgumentExpression("value")] string arg = null
#endif
        )
        {
            return IcInternal(value, label?.ToString(), memberName, lineNumber, filePath
#if NETFRAMEWORK || NETSTANDARD
#else
                , arg
#endif
            );
        }

        /// <summary>
        /// This function prints current context to the console and returns the current context as a string.
        /// Call this function directly. (e.g. ic())
        /// </summary>
        public static string ic()
        {
            // use StackTrace to avoid conflict with IceCreamTraditional
            var st = new StackTrace(new StackFrame(1, true));
            var sf = st.GetFrame(0);
            var filePath = sf?.GetFileName() ?? "";
            var lineNumber = sf?.GetFileLineNumber() ?? 0;
            var memberName = sf?.GetMethod()?.Name ?? "";

            return IcSimple(memberName, lineNumber, filePath);
        }

        /// <summary>
        /// Enable ic() and IceFormat() functions.
        /// </summary>
        public static void Enable()
        {
            Enabled = true;
        }

        /// <summary>
        /// Disable ic() and IceFormat() functions.
        /// ic() will print nothing but still returns original value; IceFormat() will return an empty string.
        /// </summary>
        public static void Disable()
        {
            Enabled = false;
        }

        /// <summary>
        /// Toggle contexts including file path, line number, timestamp, calling member name.
        /// Default is true.
        /// </summary>
        /// <param name="includeContext"></param>
        public static void SetIncludeContext(bool includeContext)
        {
            Settings.IncludeContext = includeContext;
        }

        /// <summary>
        /// Set prefix for ic() and IceFormat() functions.
        /// </summary>
        public static void SetPrefix(string prefix)
        {
            Settings.Prefix = prefix;
        }

        /// <summary>
        /// Toggle whether to use absolute path or file name only in context.
        /// Default is false.
        /// </summary>
        public static void SetUseAbsPath(bool useAbsPath)
        {
            Settings.UseAbsPath = useAbsPath;
        }

        public static void SetUseColor(bool useColor)
        {
            Settings.UseColor = useColor;
        }

        /// <summary>
        /// Set output action for ic() function.
        /// Default is Console.WriteLine.
        /// </summary>
        public static void SetOutputAction(Action<string> outputAction)
        {
            Settings.OutputAction = outputAction;
        }

        /// <summary>
        /// Set function to convert argument value to string.
        /// Default is JsonConvert.SerializeObject(value, new StringEnumConverter()).
        /// </summary>
        public static void SetArgToStringFunction(Func<object, string> argToStringFunction)
        {
            Settings.ArgToStringFunction = argToStringFunction;
        }

        /// <summary>
        /// Set console encoding. Default is Encoding.UTF8.
        /// </summary>
        public static void SetConsoleEncoding(Encoding encoding)
        {
            Settings.ConsoleEncoding = encoding;
        }

        /// <summary>
        /// Reset all settings to default.
        /// </summary>
        public static void ResetSettings()
        {
            Settings = new IceCreamSettings();
        }
    }
}
