using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Converters;

namespace icecream
{
    public static class IceCream
    {
        
        public static void printCallerFileName(this object obj)
        {
            StackTrace st = new StackTrace(new StackFrame(1, true));
            StackFrame sf = st.GetFrame(0);

            string fileName = sf.GetFileName();
            Console.WriteLine(fileName);
            Console.WriteLine(Path.GetFileName(fileName));
        }
        
        private static bool _enabled = true;
        private static string _prefix = "\ud83c\udf67|";

        private static Func<object, string> ArgToStringFunction { get; set; }
            = obj => JsonConvert.SerializeObject(obj, new StringEnumConverter());

        private static Func<string, string> OutputFunction { get; set; }
        private static bool _includeContext = true;
        private static bool _contextAbsPath = false;

        public static void ConfigureOutput(bool includeContext = true, string prefix = "\ud83c\udf67|",
            Func<string, string> outputFunc = null, Func<object, string> argFunc = null, bool absPath = false)
        {
            _prefix = prefix;
            if (outputFunc != null)
            {
                OutputFunction = outputFunc;
            }

            if (argFunc != null)
            {
                ArgToStringFunction = argFunc;
            }

            _includeContext = includeContext;
            _contextAbsPath = absPath;
        }

        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        // public static string Format(params object[] args)
        // {
        //     if (!_enabled)
        //     {
        //         return string.Empty;
        //     }
        //
        //     var context = GetContextInfo();
        //     var arguments = string.Join(" - ", args);
        //     var output = $"{_prefix} {context} - {arguments}";
        //
        //     return output;
        // }

        public static string format<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_enabled)
            {
                return string.Empty;
            }

            var valueStr = string.Empty;
            try
            {
                valueStr = ArgToStringFunction(value);
            }
            catch (Exception e)
            {
                valueStr = "ArgToStringFunction failed to serialize value, error: " + e;
            }
            
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var filePath = _contextAbsPath ? GetAbsoluteFilePath() : GetRelativeFilePath();
            var context = $"{filePath}:{lineNumber} in {memberName}() at {timestamp}";
            var output = !string.IsNullOrEmpty(label)
                ? $"{_prefix} {context} - {label}: {valueStr}"
                : $"{_prefix} {context} - {valueStr}";
            return output;
        }

        
        public static T ic<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            var output = format(value, label, memberName, lineNumber);

            if (OutputFunction != null)
            {
                try
                {
                    OutputFunction(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OutputFunction failed to process output: {output}, error: {e}");
                }
            }
            else
            {
                Console.WriteLine(output);
            }

            return value;
        }

        private static string GetRelativeFilePath()
        {
            StackTrace st = new StackTrace(new StackFrame(1, true));
            StackFrame sf = st.GetFrame(0);

            string fileName = sf.GetFileName();
            return Path.GetFileName(fileName);
        }

        private static string GetAbsoluteFilePath()
        {
            StackTrace st = new StackTrace(new StackFrame(1, true));
            StackFrame sf = st.GetFrame(0);

            string fileName = sf.GetFileName();
            return fileName;
        }
    }
}