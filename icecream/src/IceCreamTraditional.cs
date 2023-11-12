using System.Runtime.CompilerServices;
using static icecream.IceCreamCore;

namespace icecream
{
    public static class IceCreamTraditional
    {
        /// <summary>
        /// This function is like .ic() but the output is returned as a string instead of being printed to the console.
        /// Call this function after the value you want to print. (e.g. "Hello".IceFormat())
        /// A label parameter can be passed to label the output. (e.g. "Hello".IceFormat("greeting"))
        /// </summary>
        public static string IceFormat<T>(
            T value,
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
            T value,
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
    }
}