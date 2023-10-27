using NUnit.Framework;
using System;
using System.IO;
using icecream;

namespace tests
{
    [TestFixture]
    public class IceCreamTests
    {
        private StringWriter _output;
        private TextWriter _originalOutput;

        [SetUp]
        public void Setup()
        {
            // Redirect the console output for testing
            _originalOutput = Console.Out;
            _output = new StringWriter();
            Console.SetOut(_output);
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original console output
            Console.SetOut(_originalOutput);
            _output.Dispose();
        }

        // [Test]
        // public void ConfigureOutput_ShouldSetConfiguration()
        // {
        //     // Arrange
        //     IceCream.ConfigureOutput(includeContext: false, prefix: "TestPrefix");
        //
        //     // Act
        //     var outputFunction = IceCream.OutputFunction;
        //     var argToStringFunction = IceCream.ArgToStringFunction;
        //
        //     // Assert
        //     Assert.IsFalse(IceCream._includeContext);
        //     Assert.AreEqual("TestPrefix", IceCream._prefix);
        //     Assert.IsNull(outputFunction);
        //     Assert.IsNotNull(argToStringFunction);
        // }

        // [Test]
        // public void Enable_ShouldEnableLogging()
        // {
        //     // Act
        //     IceCream.Enable();
        //     bool isEnabled = IceCream._enabled;
        //
        //     // Assert
        //     Assert.IsTrue(isEnabled);
        // }

        // [Test]
        // public void Disable_ShouldDisableLogging()
        // {
        //     // Act
        //     IceCream.Disable();
        //     bool isEnabled = IceCream._enabled;
        //
        //     // Assert
        //     Assert.IsFalse(isEnabled);
        // }

        [Test]
        public void ic_ShouldWriteToConsole()
        {
            // Arrange
            IceCream.ConfigureOutput(includeContext: true, prefix: "TestPrefix");

            // Act
            var value = "Test".ic("Label");

            // Assert
            string expectedOutputPattern = @"TestPrefix tests.cs:77 in ic_ShouldWriteToConsole\(\) at \d{2}:\d{2}:\d{2}\.\d{3} - Label: ""Test""";
            string consoleOutput = _output.ToString().Trim();
            Assert.That(consoleOutput, Does.Match(expectedOutputPattern));
            Assert.That(value, Is.EqualTo("Test"));
        }
    }
}