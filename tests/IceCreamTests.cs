using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using icecream;

namespace tests
{
    [TestFixture]
    public class IceCreamTests
    {
        private StringWriter _output;
        private TextWriter _originalOutput;
        private const string _defaultPrefix = "\ud83c\udf67| ";
        private const string _defaultTimestamp = "00:00:00.000";

        private string _str;
        private int _num;
        private double _dbl;
        private bool _boolean;
        private object _obj;
        private Dictionary<string, object> _dict;
        private List<object> _list;
        private string[] _arr;
        private KeyValuePair<string, object> _kvp;
        private TestClass _testClass;
        private TestEnum _testEnum;

        private class TestClass
        {
            private int PrivateInt { get; set; } = 1;
            private string PrivateString { get; set; } = "private";
            public int PublicInt { get; set; } = 2;
            public string PublicString { get; set; } = "public";
        }

        private enum TestEnum
        {
            A,
            B,
            C
        }

        private static string UpdateTimeStamp(string str)
        {
            const string pattern = @"\d{2}:\d{2}:\d{2}\.\d{3}";
            var rgx = new System.Text.RegularExpressions.Regex(pattern);
            return rgx.Replace(str, _defaultTimestamp);
        }

        [SetUp]
        public void Setup()
        {
            // Reset the settings to default
            IceCream.ResetSettings();

            // Redirect the console output for each test
            _originalOutput = Console.Out;
            _output = new StringWriter();
            Console.SetOut(_output);

            // Arrange
            _str = "abc";
            _num = 123;
            _dbl = 123.456;
            _boolean = true;
            _obj = new { a = 1, b = "2", c = new { d = 3, e = new { f = 4 } } };
            _dict = new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", "2" },
                { "c", new { d = 3, e = new { f = 4 } } },
                { "d", new Dictionary<string, TestClass> { { "test", new TestClass() } } }
            };
            _list = new List<object> { 1, "2", new { d = 3, e = new { f = 4 } }, new TestClass() };
            _arr = new string[] { "a", "b", "c" };
            _kvp = new KeyValuePair<string, object>("a", 1);
            _testClass = new TestClass();
            _testEnum = TestEnum.A;
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original console output every time a test finishes
            _output.GetStringBuilder().Clear();
            _output.Dispose();
            Console.SetOut(_originalOutput);
        }

        [Test]
        public void ic_ShouldWriteToConsole()
        {
            // Arrange
            var anonymousObject = new { a = 1, b = "2", c = new { d = 3, e = new { f = 4 } } };
            var expectedPrefixAndContext =
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {IceCreamTestHelper.ArgName}:";
            var expectedOutputs = new[]
            {
                $"\"{_str}\"",
                $"{_num}",
                $"{_dbl}",
                $"{_boolean.ToString().ToLower()}",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}},\"d\":{{\"test\":{{\"PublicInt\":2,\"PublicString\":\"public\"}}}}}}",
                $"[1,\"2\",{{\"d\":3,\"e\":{{\"f\":4}}}},{{\"PublicInt\":2,\"PublicString\":\"public\"}}]",
                $"[\"a\",\"b\",\"c\"]",
                $"{{\"Key\":\"a\",\"Value\":1}}",
                $"{{\"PublicInt\":2,\"PublicString\":\"public\"}}",
                $"\"A\"",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}"
            };

            var inputValues = new object[]
            {
                _str, _num, _dbl, _boolean, _obj, _dict, _list, _arr,
                _kvp, _testClass, _testEnum, anonymousObject
            };

            // Act & Assert
            for (var i = 0; i < inputValues.Length; i++)
            {
                IceCreamTestHelper.DoIc(inputValues[i]);
                var consoleOutput = UpdateTimeStamp(_output.ToString().Trim());
                _output.GetStringBuilder().Clear();
                Assert.That(consoleOutput, Is.EqualTo(expectedPrefixAndContext + expectedOutputs[i]));
            }
        }

        [Test]
        public void IceFormat_ShouldReturnFormattedString()
        {
            // Arrange
            var anonymousObject = new { a = 1, b = "2", c = new { d = 3, e = new { f = 4 } } };
            var expectedPrefixAndContext =
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {IceCreamTestHelper.ArgName}:";
            var expectedOutputs = new[]
            {
                $"\"{_str}\"",
                $"{_num}",
                $"{_dbl}",
                $"{_boolean.ToString().ToLower()}",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}},\"d\":{{\"test\":{{\"PublicInt\":2,\"PublicString\":\"public\"}}}}}}",
                $"[1,\"2\",{{\"d\":3,\"e\":{{\"f\":4}}}},{{\"PublicInt\":2,\"PublicString\":\"public\"}}]",
                $"[\"a\",\"b\",\"c\"]",
                $"{{\"Key\":\"a\",\"Value\":1}}",
                $"{{\"PublicInt\":2,\"PublicString\":\"public\"}}",
                $"\"A\"",
                $"{{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}",
            };

            var inputValues = new object[]
            {
                _str, _num, _dbl, _boolean, _obj, _dict, _list, _arr,
                _kvp, _testClass, _testEnum, anonymousObject
            };

            // Act & Assert
            for (var i = 0; i < inputValues.Length; i++)
            {
                var formatted = IceCreamTestHelper.DoIceFormat(inputValues[i]);
                var output = UpdateTimeStamp(formatted.Trim());
                Assert.That(output, Is.EqualTo(expectedPrefixAndContext + expectedOutputs[i]));
            }
        }

        [Test]
        public void Ic_UseLabel_ShouldHaveLabelWriteToConsole()
        {
            var expectedOutput =
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {IceCreamTestHelper.ArgName}:\"{_str}\", label:Label";
            IceCreamTestHelper.DoIc(_str, "Label");
            var consoleOutput = UpdateTimeStamp(_output.ToString().Trim());
            _output.GetStringBuilder().Clear();
            Assert.That(consoleOutput, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Format_UseLabel_ShouldHaveLabelWriteToConsole()
        {
#if NETFRAMEWORK || NETSTANDARD
            var expectedOutput =
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - \"{_str}\"";
#else
            var expectedOutput =
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {IceCreamTestHelper.ArgName}:\"{_str}\"";
#endif
            var formatted = IceCreamTestHelper.DoIceFormat(_str, "Label");
            var consoleOutput = UpdateTimeStamp(formatted.Trim());
            Assert.That(consoleOutput, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Ic_ReturnsValue()
        {
            var value = 1.ic();
            var obj1 = new object();
            var obj2 = obj1.ic();
            var obj3 = new object();
            Assert.Multiple(() =>
            {
                Assert.That(value, Is.EqualTo(1));
                Assert.That(obj2, Is.EqualTo(obj1));
                Assert.That(obj3, Is.Not.EqualTo(obj1));
            });
        }

        [Test]
        public void ConfigureOutput_DisableContext()
        {
            // Arrange
            IceCream.SetPrefix("Prefix> ");
            IceCream.SetIncludeContext(false);

            // Act
            var x = ((1 + 1) / 2).ic("Label");

            // Assert
#if NETFRAMEWORK || NETSTANDARD
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix>2, label:Label"));
#else
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix> (1 + 1) / 2:1, label:Label"));
#endif
            Assert.That(x, Is.EqualTo(1));
        }

        [Test]
        public void ConfigureOutput_SetArgToStringFunction()
        {
            // Arrange
            IceCream.SetPrefix("Prefix>");
            IceCream.SetIncludeContext(false);
            IceCream.SetArgToStringFunction(o => $"str:{o.ToString()}");

            // Act
            var one = 1;
            one.ic("Label");

            // Assert
#if NETFRAMEWORK || NETSTANDARD
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix>str:1, label:Label"));
#else
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix>one:str:1, label:Label"));
#endif
        }

        [Test]
        public void ConfigureOutput_ArgToStringFunctionThrowsException()
        {
            // Arrange
            IceCream.SetPrefix("Prefix>");
            IceCream.SetIncludeContext(false);
            IceCream.SetArgToStringFunction(o => throw new Exception("Test"));

            // Act & Assert
            1.ic("Label");

            // Assert
#if NETFRAMEWORK || NETSTANDARD
            StringAssert.StartsWith("Prefix>ArgToStringFunction failed to serialize value, error:",
                _output.ToString().Trim());
#else
            StringAssert.StartsWith("Prefix>1:ArgToStringFunction failed to serialize value, error:",
                _output.ToString().Trim());
#endif
            _output.GetStringBuilder().Clear();
        }

        [Test]
        public void ConfigureOutput_SetOutputFunction()
        {
            // Arrange
            IceCream.SetPrefix("Prefix>");
            IceCream.SetIncludeContext(false);
            IceCream.SetOutputAction(s => Console.WriteLine($"Output:{s}"));

            // Act
            1.ic("Label");

            // Assert
#if NETFRAMEWORK || NETSTANDARD
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Output:Prefix>1, label:Label"));
#else
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Output:Prefix>1:1, label:Label"));
#endif
            _output.GetStringBuilder().Clear();
        }

        [Test]
        public void ConfigureOutput_OutputFunctionThrowsException()
        {
            // Arrange
            IceCream.SetPrefix("Prefix>");
            IceCream.SetIncludeContext(false);
            IceCream.SetOutputAction(s => throw new Exception("Test"));

            // Act & Assert
            var x = 1.ic("Label");

            // Assert
            StringAssert.StartsWith("OutputFunction failed to process output:",
                _output.ToString().Trim());
            _output.GetStringBuilder().Clear();
        }

        [Test]
        public void Ic_DisableAndEnable()
        {
            // Arrange
            IceCream.SetIncludeContext(false);

            IceCream.Disable();
            1.ic("Label");
            Assert.That(_output.ToString().Trim(), Is.EqualTo(string.Empty));
            _output.GetStringBuilder().Clear();
            IceCream.Disable();
            1.ic("Label");
            Assert.That(_output.ToString().Trim(), Is.EqualTo(string.Empty));
            _output.GetStringBuilder().Clear();
            IceCream.Enable();
            1.ic("Label");
#if NETFRAMEWORK || NETSTANDARD
            Assert.That(_output.ToString().Trim(), Is.EqualTo("{_defaultPrefix}1, label:Label"));
#else
            Assert.That(_output.ToString().Trim(), Is.EqualTo($"{_defaultPrefix}1:1, label:Label"));
#endif
            _output.GetStringBuilder().Clear();
            IceCream.Enable();
            1.ic("Label");
#if NETFRAMEWORK || NETSTANDARD
            Assert.That(_output.ToString().Trim(), Is.EqualTo("{_defaultPrefix}1, label:Label"));
#else
            Assert.That(_output.ToString().Trim(), Is.EqualTo($"{_defaultPrefix}1:1, label:Label"));
#endif
            _output.GetStringBuilder().Clear();
        }
    }
}