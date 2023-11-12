using System;
using System.Text;
using Newtonsoft.Json;
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

        public bool UseColor { get; set; } = true;

        public Encoding ConsoleEncoding { get; set; } = Encoding.UTF8;
    }
}