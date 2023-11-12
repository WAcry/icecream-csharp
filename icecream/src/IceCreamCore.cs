using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace icecream
{
    internal static class IceCreamCore
    {
        internal static bool Enabled { get; set; } = true;
        internal static IceCreamSettings Settings { get; set; } = new IceCreamSettings();

        // Colors ANSI escape codes
        private static string Black => Settings.UseColor ? "\u001b[30m" : "";
        private static string Red => Settings.UseColor ? "\u001b[31m" : "";
        private static string Green => Settings.UseColor ? "\u001b[32m" : "";
        private static string Yellow => Settings.UseColor ? "\u001b[33m" : "";
        private static string DarkBlue => Settings.UseColor ? "\u001b[34m" : "";
        private static string Purple => Settings.UseColor ? "\u001b[35m" : "";
        private static string LightBlue => Settings.UseColor ? "\u001b[36m" : "";
        private static string ResetColor => Settings.UseColor ? "\u001b[0m" : "";

        private static string BuildPrefix()
        {
            var prefixPart = Settings.Prefix ?? string.Empty;
            return prefixPart;
        }

        private static string ToPrintPrefix()
        {
            var prefixPart = Settings.Prefix ?? string.Empty;
            return prefixPart;
        }

        private static string BuildContext(
            string memberName = "",
            int lineNumber = 0,
            string filePath = "")
        {
            if (!Settings.IncludeContext) return string.Empty;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            filePath = Settings.UseAbsPath || string.IsNullOrEmpty(filePath)
                ? filePath
                : Path.GetFileName(filePath);
            var context = $"{filePath}:{lineNumber} in {memberName}() at {timestamp} ";
            return context;
        }

        private static string ToPrintContext(string memberName = "",
            int lineNumber = 0,
            string filePath = "")
        {
            if (!Settings.IncludeContext) return string.Empty;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            filePath = Settings.UseAbsPath || string.IsNullOrEmpty(filePath)
                ? filePath
                : Path.GetFileName(filePath);
            return
                $"{filePath}:" +
                $"{Green}" +
                $"{lineNumber}" +
                $"{ResetColor}" +
                $" in " +
                $"{Purple}" +
                $"{memberName}()" +
                $"{ResetColor}" +
                $" at {timestamp} ";
        }

        private static string BuildArgName(string arg)
        {
            return string.IsNullOrEmpty(arg)
                ? string.Empty
                : $"{arg}: ";
        }

        private static string ToPrintArgName(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return string.Empty;
            return
                $"{Red}" +
                $"{arg}" +
                $"{ResetColor}" +
                $": ";
        }

        private static string ArgValueToString<T>(T value)
        {
            string values;

            try
            {
                values = Settings.ArgToStringFunction == null
                    ? SerializeAndReformatAndColoring(value, false)
                    : Settings.ArgToStringFunction(value);
            }
            catch (Exception e)
            {
                values = "ArgToStringFunction failed to serialize value, error: " + e;
            }

            return values + " ";
        }

        private static string ToPrintArgValue<T>(T value)
        {
            string values;
            try
            {
                values = Settings.ArgToStringFunction == null
                    ? SerializeAndReformatAndColoring(value, Settings.UseColor)
                    : Settings.ArgToStringFunction(value);
            }
            catch (Exception e)
            {
                values = $"{Red}ArgToStringFunction failed to serialize value, error: {e}{ResetColor}";
            }

            return values + " ";
        }

        private static string BuildLabel(string label)
        {
            return string.IsNullOrEmpty(label)
                ? string.Empty
                : $"label: {label} ";
        }

        private static string ToPrintLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) return string.Empty;
            return $"{Yellow}" +
                   $"label" +
                   $"{ResetColor}" +
                   $": " +
                   $"{Yellow}" +
                   $"{label}" +
                   $"{ResetColor}" +
                   $" ";
        }

        internal static string IceFormatInternal<T>(T value, string label = null, string memberName = "",
            int lineNumber = 0, string filePath = "", string arg = null)
        {
            if (!Enabled)
            {
                return string.Empty;
            }

            var contextPart = BuildContext(memberName, lineNumber, filePath);
            var argName = BuildArgName(arg);
            var prefixPart = BuildPrefix();
            var strArgValue = ArgValueToString(value);
            var labelPart = BuildLabel(label);
            var output = $"{prefixPart}{contextPart}{argName}{strArgValue}{labelPart}".Trim();

            return output;
        }

        internal static T IcInternal<T>(T value, string label = null, string memberName = "",
            int lineNumber = 0, string filePath = "", string arg = null)
        {
            if (!Enabled)
            {
                return value;
            }

            Console.OutputEncoding = Settings.ConsoleEncoding;
            Console.InputEncoding = Settings.ConsoleEncoding;

            if (Settings.OutputAction != null)
            {
                try
                {
                    Settings.OutputAction(IceFormatInternal(value, label, memberName, lineNumber, filePath, arg));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{Red}OutputAction failed to process output: {e}{ResetColor}");
                }

                return value;
            }

            var contextPart = ToPrintContext(memberName, lineNumber, filePath);
            var argName = ToPrintArgName(arg);
            var prefixPart = ToPrintPrefix();
            var labelPart = ToPrintLabel(label);
            var strArgValue = ToPrintArgValue(value);
            var output = $"{prefixPart}{contextPart}{argName}{strArgValue}{labelPart}".Trim();
            Console.WriteLine(output);
            return value;
        }

        internal static string IcSimple(string memberName = "",
            int lineNumber = 0, string filePath = "")
        {
            if (!Enabled)
            {
                return string.Empty;
            }

            Console.OutputEncoding = Settings.ConsoleEncoding;
            Console.InputEncoding = Settings.ConsoleEncoding;
            var output = string.Empty;
            if (Settings.OutputAction != null)
            {
                try
                {
                    var contextPart = BuildContext(memberName, lineNumber, filePath);
                    var prefixPart = BuildPrefix();
                    output = $"{prefixPart}{contextPart}".Trim();
                    Settings.OutputAction(output);
                    return output;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{Red}OutputAction failed to process output: {e}{ResetColor}");
                    return output;
                }
            }
            else
            {
                var contextPart = ToPrintContext(memberName, lineNumber, filePath);
                var prefixPart = ToPrintPrefix();
                output = $"{prefixPart}{contextPart}".Trim();
                Console.WriteLine(output);
                return output;
            }
        }

        private static string SerializeAndReformatAndColoring(object obj, bool useColor)
        {
            var json = JsonConvert.SerializeObject(obj, new StringEnumConverter());
            var jToken = JToken.Parse(json);
            var list = new List<Tuple<string, string>>();
            ProcessJToken(jToken, list);

            if (!useColor)
            {
                return string.Join("", list.Select(x => x.Item1));
            }
            else
            {
                return string.Join("", list.Select(x => x.Item2 != null ? x.Item2 + x.Item1 + ResetColor : x.Item1));
            }
        }

        private static void ProcessJToken(JToken token, List<Tuple<string, string>> list)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    list.Add(new Tuple<string, string>("{", null));
                    foreach (var child in token.Children<JProperty>())
                    {
                        ProcessJToken(child, list);
                        if (child.Next == null) continue;
                        list.Add(new Tuple<string, string>(", ", null));
                    }

                    list.Add(new Tuple<string, string>("}", null));
                    break;
                case JTokenType.Array:
                    list.Add(new Tuple<string, string>("[", null));

                    var i = 0;
                    foreach (var child in token.Children())
                    {
                        ProcessJToken(child, list);
                        if (i < token.Children().Count() - 1)
                        {
                            list.Add(new Tuple<string, string>(", ", null));
                        }

                        i++;
                    }

                    list.Add(new Tuple<string, string>("]", null));
                    break;
                case JTokenType.Property:
                    // list.Add($"{DarkBlue}'{(token as JProperty).Name}'{ResetColor}: ");
                    list.Add(new Tuple<string, string>($"'{(token as JProperty).Name}'", DarkBlue));
                    list.Add(new Tuple<string, string>(": ", null));
                    ProcessJToken((token as JProperty).Value, list);
                    break;
                case JTokenType.None:
                case JTokenType.Constructor:
                case JTokenType.Comment:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                default:
                    if (token.Type == JTokenType.String)
                    {
                        list.Add(new Tuple<string, string>($"\"{token}\"", LightBlue));
                        break;
                    }

                    if (token.Type == JTokenType.Null)
                    {
                        list.Add(new Tuple<string, string>("null", LightBlue));
                        break;
                    }

                    if (token.Type == JTokenType.Boolean)
                    {
                        list.Add(new Tuple<string, string>($"{token.ToString().ToLower()}", LightBlue));
                        break;
                    }

                    list.Add(new Tuple<string, string>($"{token}", LightBlue));
                    break;
            }
        }
    }
}