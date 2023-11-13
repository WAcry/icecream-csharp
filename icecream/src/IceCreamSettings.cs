using System;
using System.Text;

namespace icecream
{
    internal class IceCreamSettings
    {
        internal bool IncludeContext { get; set; } = true;

        internal string Prefix { get; set; } = "\ud83c\udf67| ";

        internal bool UseAbsPath { get; set; } = false;

        internal Action<string> OutputAction { get; set; } = null;

        internal Func<object, string> ArgToStringFunction { get; set; } = null;

        internal bool UseColor { get; set; } = true;

        internal Encoding ConsoleEncoding { get; set; } = Encoding.UTF8;
    }
}