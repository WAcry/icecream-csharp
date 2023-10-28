using icecream;

namespace tests
{
    public static class IceCreamTestHelper
    {
        public const string FileName = "IceCreamTestHelper.cs";
        public const int IcLineNum = 15;
        public const string IcFuncName = "Ic()";
        public const int FormatLineNum = 20;
        public const string FormatFuncName = "IceFormat()";

        public static T Ic<T>(T arg, string label = null)
        {
            return label == null ? arg.ic() : arg.ic(label);
        }

        public static string IceFormat<T>(T arg, string label = null)
        {
            return label == null ? arg.IceFormat() : arg.IceFormat(label);
        }
    }
}