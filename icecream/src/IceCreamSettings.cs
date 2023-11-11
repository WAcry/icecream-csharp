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

        public Action<string> OutputAction { get; set; } = Console.WriteLine;

        public Func<object, string> ArgToStringFunction { get; set; } =
            value => JsonConvert.SerializeObject
                (value, new StringEnumConverter());

        private string StylePath { get; set; } = null;

        public Encoding ConsoleEncoding { get; set; } = Encoding.UTF8;
    }
}