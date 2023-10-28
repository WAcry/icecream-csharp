using System;
using System.Text;

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
}