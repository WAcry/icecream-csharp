using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace icecream;

internal static class IceCreamCore
{
    internal static bool Enabled { get; set; } = true;
    internal static IceCreamSettings Settings { get; set; } = new();

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
        var context = $"{filePath}:{lineNumber} in {memberName}() at {timestamp} - ";
        return context;
    }

    private static string BuildArgName(string arg)
    {
        return string.IsNullOrEmpty(arg)
            ? string.Empty
            : $"{arg}:";
    }

    private static string BuildPrefix()
    {
        var prefixPart = Settings.Prefix ?? string.Empty;
        return prefixPart;
    }

    private static string ArgValueToString<T>(T value)
    {
        string values;
        try
        {
            values = Settings.ArgToStringFunction == null
                ? JsonConvert.SerializeObject(value, new StringEnumConverter())
                : Settings.ArgToStringFunction(value);
        }
        catch (Exception e)
        {
            values = "ArgToStringFunction failed to serialize value, error: " + e;
        }

        return values;
    }

    internal static string IceFormatInternal<T>(T value, string label = null, string memberName = "",
        int lineNumber = 0, string filePath = "", string arg = null)
    {
        if (!Enabled)
        {
            return "";
        }

        var contextPart = BuildContext(memberName, lineNumber, filePath);
        var labelAndArgPart = BuildArgName(arg);
        var prefixPart = BuildPrefix();
        var output = $"{prefixPart}{contextPart}{labelAndArgPart}{ArgValueToString(value)}";
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

        var contextPart = BuildContext(memberName, lineNumber, filePath);
        var labelAndArgPart = BuildArgName(arg);
        var prefixPart = BuildPrefix();
        var labelPart = string.IsNullOrEmpty(label)
            ? string.Empty
            : $", label:{label}";
        var output = $"{prefixPart}{contextPart}{labelAndArgPart}{ArgValueToString(value)}{labelPart}";
        try
        {
            Settings.OutputAction(output);
        }
        catch (Exception e)
        {
            Console.WriteLine($"OutputFunction failed to process output: {output}, error: {e}");
        }

        return value;
    }
}