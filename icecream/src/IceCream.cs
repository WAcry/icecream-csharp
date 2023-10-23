using System;
using System.Collections.Generic;
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

        public static string Format<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_enabled)
            {
                return string.Empty;
            }
        
            var contextPart = BuildContext(memberName, lineNumber);
            var labelPart = !string.IsNullOrEmpty(label) ? $"{label}: " : string.Empty;
            var prefix = _prefix == "\ud83c\udf67|" ? "ic|" : _prefix;
            var output = $"{prefix} {contextPart}{labelPart}{GetNativeValues(value)}";
            return output;
        }

        private static string BuildContext(string memberName = "", int lineNumber = 0)
        {
            if (!_includeContext) return string.Empty;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var filePath = _contextAbsPath ? GetAbsoluteFilePath() : GetRelativeFilePath();
            var context = $"{filePath}:{lineNumber} in {memberName}() at {timestamp} - ";
            return context;
        }

        private static string GetNativeValues<T>(T value)
        {
            string values;
            try
            {
                values = ArgToStringFunction(value);
            }
            catch (Exception e)
            {
                values = "ArgToStringFunction failed to serialize value, error: " + e;
            }
            return values;
        }

        private static IEnumerable<(ConsoleColor?, string)> GetValuesWithColor<T>(T value)
        {
            var values = GetNativeValues(value);
            return Coloring.ConvertJsonIntoList(values);
        }

        public static T ic<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_enabled)
            {
                return value;
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            var contextPart = BuildContext(memberName, lineNumber);
            var labelPart = !string.IsNullOrEmpty(label) ? $"{label}: " : string.Empty;

            if (OutputFunction != null)
            {
                var output = $"{_prefix} {contextPart}{labelPart}{GetNativeValues(value)}";               
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
                Console.Write($"{_prefix} {contextPart}");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write(labelPart);
                Console.ResetColor();
                foreach (var (color, text) in GetValuesWithColor(value))
                {
                    Console.ForegroundColor = color ?? Console.ForegroundColor;
                    Console.Write(text);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            return value;
        }

        private static string GetRelativeFilePath()
        {
            var st = new StackTrace(new StackFrame(3, true));
            var sf = st.GetFrame(0);
            var fileName = sf?.GetFileName();
            return Path.GetFileName(fileName);
        }

        private static string GetAbsoluteFilePath()
        {
            var st = new StackTrace(new StackFrame(3, true));
            var sf = st.GetFrame(0);
            var fileName = sf?.GetFileName() ?? "";
            return fileName;
        }
    }
}