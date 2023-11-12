# ![Logo](https://raw.githubusercontent.com/WAcry/icecream-csharp/main/logo.png) icecream-csharp

[![NuGet version (icecream)](https://img.shields.io/nuget/v/icecream.svg?style=flat-square)](https://www.nuget.org/packages/icecream)
[![Build status](https://github.com/WAcry/icecream-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/WAcry/icecream-csharp/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-green)](https://github.com/WAcry/icecream-csharp/blob/master/LICENSE)
[![.NET](https://img.shields.io/badge/sdk.version-.NET%3E5.0%20%7C%20.NET%20Core%203.1%20%7C%20.NET%20Standard%202.0%20%7C%20.NET%20Framework%204.5-blue)](https://dotnet.microsoft.com/en-us/)

### IceCream — Never use Console to debug again

This C# library is inspired by [icecream-python](https://github.com/gruns/icecream).

Do you ever use `Console.WriteLine()` to debug your code? Of course you
do. IceCream, or `ic` for short, makes print debugging a little sweeter.

`.ic()` is like `print()` in Python, except it's better:

1. **Detailed Printing**: IceCream prints not only values but also contextual information, including the **filename,
   timestamp, line number, label, argument name, and parent function.
2. **Redesigned for C#**: The tool has been redesigned to work with C#.
3. **Simplicity**: IceCream is designed for simplicity and is 60% faster to use compared to other debugging tools.
4. **Flexible Configuration**: You can configure various settings in IceCream to customize your debugging output according
   to your specific needs.
5. **Output Customization**: You can further customize the debugging output by adding labels, prefixes, and more to suit
   your preferences.
6. **Readability**: IceCream is syntax colored and reformatted to make it easier to read.

IceCream is well tested, [permissively licensed](LICENSE), and
supports .NET 5.0, .NET Core 3.1, .NET Standard 2.0, and .NET Framework 4.5.

### Install and Import

First, add the library to your project package references in your `.csproj` file.

```
<PackageReference Include="icecream"/>
```

Or, use the dotnet CLI.

```
$ dotnet add package icecream
```

After the package is installed, import it in your code.

```csharp
using static icecream.IceCream;
```

### Quick Start

```csharp
int foo(int x) => x + 1;
var x = 1;
var dict = new Dictionary<object, object> { { "a", 1 }, { 2, "b" } };

ic();
x.ic();
foo(foo(2)).ic("call foo twice");
var y = foo(x.ic("ic() returns the original value")) * 2;
(y, dict).ic("multiple values");
```

Prints as:

![Coloring](https://raw.githubusercontent.com/WAcry/icecream-csharp/main/coloring.png)

As you may noticed, you can add `.ic()` almost ANYWHERE and have no impact on the code logic because it returns the original
value. 

I believe `obj.ic()` is more elegant than `ic(obj)`. For example, You can easily add them with one mouse click, 
and remove them all with `Replace All` in your IDE. 

**However, if you really want to use the traditional way `ic(obj)` as 
in icecream-python, you can still do it by `using static icecream.IceCreamTraditional;`.**

### Print Anything

We use `JsonConvert.SerializeObject()` to convert the object to a string in default. It is
powerful and is able to print
much more types than `JsonSerializer.Serialize()`. You can define your own `ArgToStringFunction` to parse the object to
a string in your own way too.

```csharp
str = "abc";
num = 123;
dbl = 123.456;
boolean = true;
obj = new { a = 1, b = "2", c = new { d = 3, e = new { f = 4 } } };
dict = new Dictionary<string, object>
{
    { "a", 1 },
    { "b", "2" },
    { "c", new { d = 3, e = new { f = 4 } } },
    { "d", new Dictionary<string, TestClass> { { "test", new TestClass() } } }
};
list = new List<object> { 1, "2", new { d = 3, e = new { f = 4 } }, new TestClass() };
arr = new string[] { "a", "b", "c" };
kvp = new KeyValuePair<string, object>("a", 1);
tuple = (1, 3.14f, true, new TestClass());
testClass = new TestClass();
testEnum = TestEnum.A;
```

```
🍧| Program.cs:1 in Main() at 00:00:00.000 str: "abc"
🍧| Program.cs:2 in Main() at 00:00:00.000 num: 123
🍧| Program.cs:3 in Main() at 00:00:00.000 dbl: 123.456
🍧| Program.cs:4 in Main() at 00:00:00.000 boolean: true
🍧| Program.cs:5 in Main() at 00:00:00.000 obj: {'a': 1, 'b': "2", 'c': {'d': 3, 'e': {'f': 4}}}
🍧| Program.cs:6 in Main() at 00:00:00.000 dict: {'a': 1, 'b': "2", 'c': {'d': 3, 'e': {'f': 4}}, 'd': {'test': {'a': 1, 'b': "2"}}}
🍧| Program.cs:7 in Main() at 00:00:00.000 list: [1, "2", {'d': 3, 'e': {'f': 4}}, {'a': 1, 'b': "2"}]
🍧| Program.cs:8 in Main() at 00:00:00.000 arr: ["a", "b", "c"]
🍧| Program.cs:9 in Main() at 00:00:00.000 kvp: {'Key': "a", 'Value': 1}
🍧| Program.cs:10 in Main() at 00:00:00.000 tuple: {'Item1': 1, 'Item2': 3.14, 'Item3': true, 'Item4': {'a': 1, 'b': "2"}}
🍧| Program.cs:11 in Main() at 00:00:00.000 testClass: {'a': 1, 'b': "2"}
🍧| Program.cs:12 in Main() at 00:00:00.000 testEnum: "A"
```

Due to the high compatibility of different versions of JsonConvert, and the security risks associated with older versions, it is currently required to use JsonConvert version 13 and above. If you have a specific requirement to use an older version of JsonConvert, you can compile this library yourself, and it should generally require no modifications. However, it is recommended to use version 9 or above.

### Logging

`.IceFormat()` is like `.ic()` but the output is returned as a string instead

```csharp
var str = "hello";
logger.LogInfo(str.IceFormat());
```

### Enable/Disable

```csharp
IceCream.Enable(); // Enable IceCream
IceCream.Disable(); // Disable IceCream
```

### Configuration

Here's a overview of the settings:

```csharp
class IceCreamSettings
{
  bool IncludeContext { get; set; } = true;
  string Prefix { get; set; } = "\ud83c\udf67| ";
  bool UseAbsPath { get; set; } = false;
  Action<string> OutputAction { get; set; } = null;
  Func<object, string> ArgToStringFunction { get; set; } = null;
  Encoding ConsoleEncoding { get; set; } = Encoding.UTF8;
  bool UseColor { get; set; } = true;
}
 ```

1. `IncludeContext` (default: `true`): Whether to include context information (line number, parent function, etc.) in
   the output.
2. `Prefix` (default: `🍧| `): The prefix of the output. The default icon is shaved ice instead of soft ice cream, because shaved ice is more sharp:)
3. `UseAbsPath` (default: `false`): Whether to use absolute path of the file or the file name only.
4. `OutputAction` (default: reformatting, coloring, and printing action based on JsonConvert): The function handling the output.
5. `ArgToStringFunction` (default: `obj => JsonConvert.SerializeObject(obj, new StringEnumConverter())`): The function
   converting the object to a string.
6. `ConsoleEncoding` (default: `Encoding.UTF8`): The encoding of the output.
7. `UseColor` (default: `true`): Whether to use color in the output.

You can use `IceCream.SetXxx(newValue)` (e.g. `IceCream.SetPrefix("ic> ")`) to set a single setting.
You can use `IceCream.ResetSettings()` to reset all settings to default.

### Framework

`>.NET Core 3.1` and `>.NET 5.0` are suggested. `NET Standard 2.0` and `.NET Framework 4.5` are also supported, 
but they may not be able to print the arguments' name. (e.g. `x.ic()` may print `1` instead of `x:1`)

### IceCream in Other Languages

Delicious IceCream should be enjoyed in every language.

- Python: [icecream](https://github.com/gruns/icecream)
- Dart: [icecream](https://github.com/HallerPatrick/icecream)
- Rust: [icecream-rs](https://github.com/ericchang00/icecream-rs)
- Node.js: [node-icecream](https://github.com/jmerle/node-icecream)
- C++: [IceCream-Cpp](https://github.com/renatoGarcia/icecream-cpp)
- C99: [icecream-c](https://github.com/chunqian/icecream-c)
- PHP: [icecream-php](https://github.com/ntzm/icecream-php)
- Go: [icecream-go](https://github.com/WAY29/icecream-go)
- Ruby: [Ricecream](https://github.com/nodai2hITC/ricecream)
- Java: [icecream-java](https://github.com/Akshay-Thakare/icecream-java)
- R: [icecream](https://github.com/lewinfox/icecream)
- Lua: [icecream-lua](https://github.com/wlingze/icecream-lua)
- Clojure(Script): [icecream-cljc](https://github.com/Eigenbahn/icecream-cljc)
- Bash: [IceCream-Bash](https://github.com/jtplaarj/IceCream-Bash)
