<h1 align="center">
  <img src="logo.svg" width="220px" height="370px" alt="icecream">
</h1>

<p align="center">
  <a href="https://www.nuget.org/packages/icecream"><img src="https://img.shields.io/badge/nuget-v1.0.0-blue"></a>
  <a href="https://github.com/WAcry/icecream-csharp/actions/workflows/build-and-test.yml"><img src="https://github.com/gruns/icecream/actions/workflows/ci.yml/badge.svg"></a>
  <a href="https://github.com/WAcry/icecream-csharp/blob/master/LICENSE.md"><img src="https://img.shields.io/badge/license-MIT-green"></a>
  <a href="https://dotnet.microsoft.com/en-us/"><img src="https://img.shields.io/badge/.net-st2|3|4|5|6|7-blue"></a>
</p>

### IceCream — Never use print() to debug again

Do you ever use `Console.WriteLine()` to debug your code? Of course you
do. IceCream, or `ic` for short, makes print debugging a little sweeter.

`ic()` is like `print()` in Python, except it's better:

1. It prints not only values but also context including the filename, timestamp,
   line number, and parent function (optionally).
2. It's 60% faster to type.
3. Redesigned for C#.
4. Output is syntax highlighted.

IceCream is well tested, [permissively licensed](LICENSE.txt), and
supports mostly all versions of .NET.

### Install and Import

First, install IceCream with NuGet.

```
$ dotnet add package icecream
```

Then, import it in your code.

```csharp
using icecream;
```

### Quick Start

```csharp
"Hello World".ic("Label");
var x = 12.ic() + 2;
x.ic("x");
```

Prints as:

```
🍧| Program.cs:1 in Main() at 06:33:13.723 - Label: "Hello World"
🍧| Program.cs:2 in Main() at 06:33:13.761 - 12
🍧| Program.cs:3 in Main() at 06:33:13.766 - x: 14
```

As you can see, you can add `.ic()` almost ANYWHERE and have no impact on the code logic because it returns the original
value.

In Python, `ic(foo(123))` could print something like `ic| foo(123): 456`. However, it's impossible to do that in
Java or C# because of the way the language is designed. In this case I believe `obj.ic()` is more elegant
than `ic(obj)`.
You can easily add them and remove them all with `Replace All` in your IDE.

Usually, you don't need to add a label to the output because the context information is already enough.
While, you can still add an optional label to the output by passing a string as `.ic(label)` like the first example.

### Print Anything With Color

We use `JsonConver.SerializeObject()` to convert the object to a string in default. It is powerful and is able to print
much more types than
`JsonSerializer.Serialize()`. You can define your own `ArgToStringFunction` to parse the object to a string in your own
way too.

```csharp
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
_tuple = (1, 3.14f, true, new TestClass());
_testClass = new TestClass();
_testEnum = TestEnum.A;
```

```
🍧| Program.cs:15 in Main() at 06:52:17.146 - "abc"
🍧| Program.cs:15 in Main() at 06:52:17.189 - 123
🍧| Program.cs:15 in Main() at 06:52:17.192 - 123.456
🍧| Program.cs:15 in Main() at 06:52:17.193 - true
🍧| Program.cs:15 in Main() at 06:52:17.194 - {"a": 1, "b": "2", "c": {"d": 3, "e": {"f": 4}}}
🍧| Program.cs:15 in Main() at 06:52:17.224 - {"a": 1, "b": "2", "c": {"d": 3, "e": {"f": 4}}, "d": {"test": {"PublicInt": 2, "PublicString": "public"}}}
🍧| Program.cs:15 in Main() at 06:52:17.230 - [1, "2", {"d": 3, "e": {"f": 4}}, {"PublicInt": 2, "PublicString": "public"}]
🍧| Program.cs:15 in Main() at 06:52:17.232 - ["a", "b", "c"]
🍧| Program.cs:15 in Main() at 06:52:17.233 - {"Key": "a", "Value": 1}
🍧| Program.cs:15 in Main() at 06:52:17.236 - {"Item1": 1, "Item2": 3.14, "Item3": true, "Item4": {"PublicInt": 2, "PublicString": "public"}}
🍧| Program.cs:15 in Main() at 06:52:17.238 - {"PublicInt": 2, "PublicString": "public"}
🍧| Program.cs:15 in Main() at 06:52:17.238 - "A"
```

They don't have syntax color in this README document, but they do in your console. 😃

### Logging

`.IceFormat()` is like `ic()` but the output is returned as a string instead

```csharp
var str = "hello".IceFormat();
```

`str` is now `🍧| Program.cs:1 in Main() at 06:33:13.723 - "hello"`.

### Enable/Disable

```csharp
IceCream.Enable(); // Enable IceCream
IceCream.Disable(); // Disable IceCream
```

### Configuration

Here's a overview of the settings:
```csharp
 public class IceCreamSettings
 {
     public bool IncludeContext { get; set; } = true;
     public string Prefix { get; set; } = "\ud83c\udf67| ";
     public bool UseAbsPath { get; set; } = false;
     public Action<string> OutputAction { get; set; } = null;
     public Func<object, string> ArgToStringFunction { get; set; } = null;
     public ConsoleColor? LabelColor { get; set; } = ConsoleColor.DarkBlue;
     public ConsoleColor? FieldColor { get; set; } = ConsoleColor.DarkRed;
     public ConsoleColor? ValueColor { get; set; } = ConsoleColor.DarkCyan;
     public Encoding Encoding { get; set; } = Encoding.UTF8;
 }
 ```

Use `IceCream.Configure(IceCreamSettings settings)` to configure IceCream. You can only set the properties you want to
change, and the rest will be set to default values. Also, `IceCream.Configure()` resets all settings to default values.
                                                              
1. `IncludeContext` (default: `true`): Whether to include context information in the output.
2. `Prefix` (default: `🍧| `): The prefix of the output.
3. `UseAbsPath` (default: `false`): Whether to use absolute path of the file or the file name only.
4. `OutputAction` (default: `null`): The action to output the result. If it is `null`, the result will be output to
   `Console.WriteLine() with the color set in `LabelColor`, `FieldColor` and `ValueColor`.
5. `ArgToStringFunction` (default: `obj => JsonConvert.SerializeObject(obj, new StringEnumConverter())`): The function
  converting the object to a string. If it is `null`, the default function will be used.
6. `LabelColor` (default: `ConsoleColor.DarkBlue`): The color of the label.
7. `FieldColor` (default: `ConsoleColor.DarkRed`): The color of the field.
8. `ValueColor` (default: `ConsoleColor.DarkCyan`): The color of the value.
9. `Encoding` (default: `Encoding.UTF8`): The encoding of the output.

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