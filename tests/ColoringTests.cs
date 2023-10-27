using System;
using System.Collections.Generic;
using System.Linq;
using icecream;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class ColoringTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConvertJsonIntoList_ShouldConvertJsonToColorList()
        {
            // Arrange
            const string json = "{\"string\":\"abc\",\"int\":30,\"bool\":true,\"null\":null,\"array\":[1,2,3],\"object\":{\"num\":1,\"sub_obj\":{\"num\":2}}}";
            var expectedList = new List<(ConsoleColor?, string)>
            {
                (null, "{"),
                (ConsoleColor.DarkRed, "\"string\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "\"abc\""),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"int\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "30"),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"bool\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "true"),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"null\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "null"),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"array\""),
                (null, ": "),
                (null, "["),
                (ConsoleColor.DarkCyan, "1"),
                (null, ", "),
                (ConsoleColor.DarkCyan, "2"),
                (null, ", "),
                (ConsoleColor.DarkCyan, "3"),
                (null, "]"),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"object\""),
                (null, ": "),
                (null, "{"),
                (ConsoleColor.DarkRed, "\"num\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "1"),
                (null, ", "),
                (ConsoleColor.DarkRed, "\"sub_obj\""),
                (null, ": "),
                (null, "{"),
                (ConsoleColor.DarkRed, "\"num\""),
                (null, ": "),
                (ConsoleColor.DarkCyan, "2"),
                (null, "}"),
                (null, "}"),
                (null, "}"),
            };

            // Act
            var result = Coloring.ConvertJsonIntoList(json).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(expectedList.Count));
            for (var i = 0; i < expectedList.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result[i].Item1, Is.EqualTo(expectedList[i].Item1));
                    Assert.That(result[i].Item2, Is.EqualTo(expectedList[i].Item2));
                });
            }
        }

        [Test]
        public void ConvertJsonIntoList_WithFieldAndValueColor_ShouldApplyColors()
        {
            // Arrange
            const string json = "{\"name\":\"John\",\"age\":30}";
            const ConsoleColor fieldColor = ConsoleColor.Blue;
            const ConsoleColor valueColor = ConsoleColor.Yellow;
            var expectedList = new List<(ConsoleColor?, string)>
            {
                (null, "{"),
                (fieldColor, "\"name\""),
                (null, ": "),
                (valueColor, "\"John\""),
                (null, ", "),
                (fieldColor, "\"age\""),
                (null, ": "),
                (valueColor, "30"),
                (null, "}"),
            };

            // Act
            var result = Coloring.ConvertJsonIntoList(json, fieldColor, valueColor).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(expectedList.Count));
            for (var i = 0; i < expectedList.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result[i].Item1, Is.EqualTo(expectedList[i].Item1));
                    Assert.That(result[i].Item2, Is.EqualTo(expectedList[i].Item2));
                });
            }
        }
    }
}
