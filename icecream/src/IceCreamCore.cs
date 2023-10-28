using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace icecream
{
    internal static class IceCreamCore
    {
        internal static IceCreamSettings _settings = new IceCreamSettings();
        internal static bool _enabled = true;

        private static string BuildContext(string memberName = "", int lineNumber = 0, string filePath = "")
        {
            if (!_settings.IncludeContext) return string.Empty;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            filePath = _settings.UseAbsPath ? filePath : Path.GetFileName(filePath);
            var context = $"{filePath}:{lineNumber} in {memberName}() at {timestamp} - ";
            return context;
        }

        private static string BuildLabel(string label)
        {
            var labelPart = label != null ? $"{label}: " : string.Empty;
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
                    ? JsonConvert.SerializeObject(value, new StringEnumConverter())
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
            return IceCreamColoring.ConvertJsonIntoList(values, _settings.FieldColor, _settings.ValueColor);
        }

        internal static string IceFormatInternal<T>(T value, string label = null, string memberName = "",
            int lineNumber = 0, string filePath = "")
        {
            if (!_enabled)
            {
                return string.Empty;
            }

            var contextPart = BuildContext(memberName, lineNumber, filePath);
            var labelPart = BuildLabel(label);
            var prefixPart = BuildPrefix();
            var output = $"{prefixPart}{contextPart}{labelPart}{GetNativeValues(value)}";
            return output;
        }

        internal static T IcInternal<T>(T value, string label = null, string memberName = "",
            int lineNumber = 0, string filePath = "")
        {
            if (!_enabled)
            {
                return value;
            }

            Console.OutputEncoding = _settings.Encoding;
            Console.InputEncoding = _settings.Encoding;

            var contextPart = BuildContext(memberName, lineNumber, filePath);
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
    }
}