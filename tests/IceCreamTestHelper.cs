using icecream;

namespace tests
{
    internal static class IceCreamTestHelper
    {
        public const string FileName = "IceCreamTestHelper.cs";
        public const string ArgName = "Wrapper(arg)";
        public const int IcLineNum = 16;
        public const string IcFuncName = "DoIc()";
        public const int FormatLineNum = 21;
        public const string FormatFuncName = "DoIceFormat()";

        internal static T DoIc<T>(T arg, string label = null)
        {
            return label == null ? Wrapper(arg).ic() : Wrapper(arg).ic(label);
        }

        internal static string DoIceFormat<T>(T arg, string label = null)
        {
            return label == null ? Wrapper(arg).IceFormat() : Wrapper(arg).IceFormat(label);
        }

        private static T Wrapper<T>(T arg)
        {
            return arg;
        }
    }
}