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
    public class IceCreamSettings
    {
        public bool IncludeContext { get; set; } = true;
        public string CustomPrefix { get; set; } = null;
        public bool AbsPath { get; set; } = false;
        public Func<string, string> OutputFunction { get; set; } = null;
        public Func<object, string> ArgToStringFunction { get; set; } = null;
        public ConsoleColor? LabelColor { get; set; } = null;
        public ConsoleColor? FieldColor { get; set; } = null;
        public ConsoleColor? ValueColor { get; set; } = null;
        public Encoding Encoding { get; set; } = null;
    }

    public static class IceCream
    {
        private static bool _enabled = true;
        private const string _defaultEmojiPrefix = "\ud83c\udf67| ";
        private static bool _includeContext = true;
        private static string _customPrefix = null;
        private static bool _contextAbsPath = false;
        private static ConsoleColor _labelColor = ConsoleColor.DarkBlue;
        private static ConsoleColor _fieldColor = ConsoleColor.DarkRed;
        private static ConsoleColor _valueColor = ConsoleColor.DarkCyan;
        private static Encoding _encoding = Encoding.UTF8;

        private static Func<object, string> _argToStringFunction
            = obj => JsonConvert.SerializeObject(obj, new StringEnumConverter());

        private static Func<string, string> _outputFunction { get; set; } = null;

        public static void ConfigureOutput(IceCreamSettings settings)
        {
            _includeContext = settings?.IncludeContext ?? _includeContext;
            _customPrefix = settings?.CustomPrefix ?? _customPrefix;
            _contextAbsPath = settings?.AbsPath ?? _contextAbsPath;
            _outputFunction = settings?.OutputFunction ?? _outputFunction;
            _argToStringFunction = settings?.ArgToStringFunction ?? _argToStringFunction;
            _labelColor = settings?.LabelColor ?? _labelColor;
            _fieldColor = settings?.FieldColor ?? _fieldColor;
            _valueColor = settings?.ValueColor ?? _valueColor;
            _encoding = settings?.Encoding ?? _encoding;
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
            var labelPart = BuildLabel(label);
            var prefixPart = BuildPrefix();
            var output = $"{prefixPart}{contextPart}{labelPart}{GetNativeValues(value)}";
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

        private static string BuildLabel(string label)
        {
            var labelPart = !string.IsNullOrEmpty(label) ? $"{label}: " : string.Empty;
            return labelPart;
        }

        private static string BuildPrefix()
        {
            var prefixPart = _customPrefix ?? _defaultEmojiPrefix;
            return prefixPart;
        }

        private static string GetNativeValues<T>(T value)
        {
            string values;
            try
            {
                values = _argToStringFunction(value);
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
            return Coloring.ConvertJsonIntoList(values, _fieldColor, _valueColor);
        }

        public static T ic<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_enabled)
            {
                return value;
            }

            Console.OutputEncoding = _encoding;
            Console.InputEncoding = _encoding;

            var contextPart = BuildContext(memberName, lineNumber);
            var labelPart = BuildLabel(label);
            var prefixPart = BuildPrefix();

            if (_outputFunction != null)
            {
                var output = $"{prefixPart}{contextPart}{labelPart}{GetNativeValues(value)}";
                try
                {
                    _outputFunction(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OutputFunction failed to process output: {output}, error: {e}");
                }
            }
            else
            {
                // Write to console with colors thread-safely
                lock (Console.Out)
                {
                    Console.Write($"{prefixPart}{contextPart}");
                    Console.ForegroundColor = _labelColor;
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