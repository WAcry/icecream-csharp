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
        private (int, float, bool, TestClass) _tuple;
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
            IceCream.Configure();

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
                { { "a", 1 }, 
                    { "b", "2" }, 
                    { "c", new { d = 3, e = new { f = 4 } } }, 
                    { "d", new Dictionary<string, TestClass> { { "test", new TestClass() } } } };
            _list = new List<object> { 1, "2", new { d = 3, e = new { f = 4 } }, new TestClass() }; 
            _arr = new string[] { "a", "b", "c" };
            _kvp = new KeyValuePair<string, object>("a", 1);
            _tuple = (1, 3.14f, true, new TestClass());
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
            var expectedOutputs = new[]
            {
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - \"{_str}\"",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {_num}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {_dbl}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {_boolean.ToString().ToLower()}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"a\": 1, \"b\": \"2\", \"c\": {{\"d\": 3, \"e\": {{\"f\": 4}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"a\": 1, \"b\": \"2\", \"c\": {{\"d\": 3, \"e\": {{\"f\": 4}}}}, \"d\": {{\"test\": {{\"PublicInt\": 2, \"PublicString\": \"public\"}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - [1, \"2\", {{\"d\": 3, \"e\": {{\"f\": 4}}}}, {{\"PublicInt\": 2, \"PublicString\": \"public\"}}]",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - [\"a\", \"b\", \"c\"]",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"Key\": \"a\", \"Value\": 1}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"Item1\": 1, \"Item2\": 3.14, \"Item3\": true, \"Item4\": {{\"PublicInt\": 2, \"PublicString\": \"public\"}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"PublicInt\": 2, \"PublicString\": \"public\"}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - {{\"a\": 1, \"b\": \"2\", \"c\": {{\"d\": 3, \"e\": {{\"f\": 4}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - \"A\""
            };

            var inputValues = new object[]
            {
                _str, _num, _dbl, _boolean, _obj, _dict, _list, _arr, _kvp, _tuple, _testClass, anonymousObject, _testEnum
            };

            // Act & Assert
            for (var i = 0; i < inputValues.Length; i++)
            {
                IceCreamTestHelper.Ic(inputValues[i]);
                var consoleOutput = UpdateTimeStamp(_output.ToString().Trim());
                _output.GetStringBuilder().Clear();
                Assert.That(consoleOutput, Is.EqualTo(expectedOutputs[i]));
            }
        }
        
        [Test]
        public void IceFormat_ShouldReturnFormattedString()
        {
            // Arrange
            var anonymousObject = new { a = 1, b = "2", c = new { d = 3, e = new { f = 4 } } };
            var expectedOutputs = new[]
            {
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - \"{_str}\"",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {_num}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {_dbl}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {_boolean.ToString().ToLower()}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}},\"d\":{{\"test\":{{\"PublicInt\":2,\"PublicString\":\"public\"}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - [1,\"2\",{{\"d\":3,\"e\":{{\"f\":4}}}},{{\"PublicInt\":2,\"PublicString\":\"public\"}}]",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - [\"a\",\"b\",\"c\"]",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"Key\":\"a\",\"Value\":1}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"Item1\":1,\"Item2\":3.14,\"Item3\":true,\"Item4\":{{\"PublicInt\":2,\"PublicString\":\"public\"}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"PublicInt\":2,\"PublicString\":\"public\"}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - {{\"a\":1,\"b\":\"2\",\"c\":{{\"d\":3,\"e\":{{\"f\":4}}}}}}",
                $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - \"A\""
            };

            var inputValues = new object[]
            {
                _str, _num, _dbl, _boolean, _obj, _dict, _list, _arr, _kvp, _tuple, _testClass, anonymousObject, _testEnum
            };

            // Act & Assert
            for (var i = 0; i < inputValues.Length; i++)
            {
                var formatted = IceCreamTestHelper.IceFormat(inputValues[i]);
                var output = UpdateTimeStamp(formatted.Trim());
                Assert.That(output, Is.EqualTo(expectedOutputs[i]));
            }
        }

        [Test]
        public void Ic_UseLabel_ShouldHaveLabelWriteToConsole()
        {
            var expectedOutput = $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.IcLineNum} in {IceCreamTestHelper.IcFuncName} at {_defaultTimestamp} - Label: \"{_str}\"";
            IceCreamTestHelper.Ic(_str, "Label");
            var consoleOutput = UpdateTimeStamp(_output.ToString().Trim());
            _output.GetStringBuilder().Clear();
            Assert.That(consoleOutput, Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Format_UseLabel_ShouldHaveLabelWriteToConsole()
        {
            var expectedOutput = $"{_defaultPrefix}{IceCreamTestHelper.FileName}:{IceCreamTestHelper.FormatLineNum} in {IceCreamTestHelper.FormatFuncName} at {_defaultTimestamp} - Label: \"{_str}\"";
            var formatted = IceCreamTestHelper.IceFormat(_str, "Label");
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
            var iceCreamSettings = new IceCreamSettings
            {
                IncludeContext = false,
                Prefix = "Prefix>"
            };
            IceCream.Configure(iceCreamSettings);

            // Act
            1.ic("Label");

            // Assert
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix>Label: 1"));
        }
        
        [Test]
        public void ConfigureOutput_SetArgToStringFunction()
        {
            // Arrange
            var iceCreamSettings = new IceCreamSettings
            {
                Prefix = "Prefix>",
                IncludeContext = false,
                ArgToStringFunction = o => $"str:{o.ToString()}"
            };
            IceCream.Configure(iceCreamSettings);

            // Act
            1.ic("Label");

            // Assert
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Prefix>Label: str:1"));
        }

        [Test]
        public void ConfigureOutput_ArgToStringFunctionThrowsException()
        {
            // Arrange
            var iceCreamSettings = new IceCreamSettings
            {
                Prefix = "Prefix>",
                IncludeContext = false,
                ArgToStringFunction = o => throw new Exception("Test")
            };
            IceCream.Configure(iceCreamSettings);
        
            // Act & Assert
            1.ic("Label");
        
            // Assert
            StringAssert.StartsWith("Prefix>Label: ArgToStringFunction failed to serialize value, error:",
                _output.ToString().Trim());
            _output.GetStringBuilder().Clear();
        }

        [Test]
        public void ConfigureOutput_SetOutputFunction()
        {
            // Arrange
            var iceCreamSettings = new IceCreamSettings
            {
                Prefix = "Prefix>",
                IncludeContext = false,
                OutputAction = s => Console.Write("Output:" + s)
            };
            IceCream.Configure(iceCreamSettings);
        
            // Act
            1.ic("Label");
        
            // Assert
            Assert.That(_output.ToString().Trim(), Is.EqualTo("Output:Prefix>Label: 1"));
            _output.GetStringBuilder().Clear();
        }
        
        [Test]
        public void ConfigureOutput_OutputFunctionThrowsException()
        {
            // Arrange
            var iceCreamSettings = new IceCreamSettings
            {
                Prefix = "Prefix>",
                IncludeContext = false,
                OutputAction = s => throw new Exception("Test")
            };
            IceCream.Configure(iceCreamSettings);
        
            // Act & Assert
            1.ic("Label");
        
            // Assert
            StringAssert.StartsWith("OutputFunction failed to process output:",
                _output.ToString().Trim());
            _output.GetStringBuilder().Clear();
        }
        
        [Test]
        public void Ic_DisableAndEnable()
        {
            // Arrange
            var iceCreamSettings = new IceCreamSettings
            {
                IncludeContext = false,
            };
            IceCream.Configure(iceCreamSettings);

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
            Assert.That(_output.ToString().Trim(), Is.EqualTo($"{_defaultPrefix}Label: 1"));
            _output.GetStringBuilder().Clear();
            IceCream.Enable();
            1.ic("Label");
            Assert.That(_output.ToString().Trim(), Is.EqualTo($"{_defaultPrefix}Label: 1"));
            _output.GetStringBuilder().Clear();
        }
    }
}