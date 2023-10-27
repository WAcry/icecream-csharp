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
        public string Prefix { get; set; } = "\ud83c\udf67| ";
        public bool UseAbsPath { get; set; } = false;
        public Action<string> OutputAction { get; set; } = null;
        public Func<object, string> ArgToStringFunction { get; set; } = null;
        public ConsoleColor? LabelColor { get; set; } = ConsoleColor.DarkBlue;
        public ConsoleColor? FieldColor { get; set; } = ConsoleColor.DarkRed;
        public ConsoleColor? ValueColor { get; set; } = ConsoleColor.DarkCyan;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }

    public static class IceCream
    {
        private static bool _enabled = true;
        private static IceCreamSettings _settings = new IceCreamSettings();

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

        public static string IceFormat<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
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
            if (!_settings.IncludeContext) return string.Empty;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var filePath = _settings.UseAbsPath ? GetAbsoluteFilePath() : GetRelativeFilePath();
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
            var prefixPart = _settings.Prefix ?? string.Empty;
            return prefixPart;
        }

        private static string GetNativeValues<T>(T value)
        {
            string values;
            try
            {
                values = _settings.ArgToStringFunction == null
                    ?  JsonConvert.SerializeObject(value, new StringEnumConverter())
                    : _settings.ArgToStringFunction(value);
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
            return Coloring.ConvertJsonIntoList(values, _settings.FieldColor, _settings.ValueColor);
        }

        public static T ic<T>(this T value, string label = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_enabled)
            {
                return value;
            }

            Console.OutputEncoding = _settings.Encoding;
            Console.InputEncoding = _settings.Encoding;

            var contextPart = BuildContext(memberName, lineNumber);
            var labelPart = BuildLabel(label);
            var prefixPart = BuildPrefix();

            if (_settings.OutputAction != null || _settings.ArgToStringFunction != null)
            {
                var output = $"{prefixPart}{contextPart}{labelPart}{GetNativeValues(value)}";
                if (_settings.OutputAction != null)
                {
                    try
                    {
                        _settings.OutputAction(output);
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
            }
            else
            {
                // Write to console with colors thread-safely
                lock (Console.Out)
                {
                    Console.Write($"{prefixPart}{contextPart}");
                    Console.ForegroundColor = _settings.LabelColor ?? Console.ForegroundColor;
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