# FixedStrings [![ci](https://github.com/xoofx/FixedStrings/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/xoofx/FixedStrings/actions/workflows/ci.yml) [![NuGet](https://img.shields.io/nuget/v/FixedStrings.svg)](https://www.nuget.org/packages/FixedStrings/)

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/xoofx/FixedStrings/main/img/FixedStrings.png">

FixedStrings provides a zero allocation valuetype based fixed string implementation with the following size 8/16/32/64.

```c#
// Zero allocation!
FixedString16 str = $"HelloWorld {DateTime.Now.Year}";
// Prints "HelloWorld 2023"
Console.Out.WriteLine(str.AsSpan());
```

## Features

- Zero allocation via `FixedString8`, `FixedString16`, `FixedString32` and `FixedString64` structs.     
- Compatible with `net7.0`+

## User Guide

### Why FixedStrings?

Many modern applications are suffering from lots of allocations on the heap, and it is very frequent to see dozen of thousands of strings allocated on the managed heap. Having all these managed objects around is creating a lot more pressure on the GC, they can be more scattered in memory.

In many scenarios, you might be able to co-locate such strings closer to the class that are using them, but you might be able also to refine the memory requirement/usage for these strings.

For instance, on a 64 bit system, **a single allocation of an empty managed string on the heap will take 24 bytes + 8 bytes for the reference to it. That's 32 bytes for an empty string!** See [details on sharplab.io](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmABgFgAoUgRmoEk8BnABxjAwDoAJGAQ1YAKAEQiAlAG5GLdp14DhInhOlUmbDtz6DRfADb6IAdQiqgA=).

With a fixed string, like `FixedString8`, you get 7 characters and it takes only 16 bytes or a `FixedString16` would give similarly 15 characters while taking only 32 bytes!

As you can see in the benchmarks below, by avoiding an allocation to the heap, **FixedStrings can be 2x as fast as creating them on the heap**.

### How does it work?

FixedStrings is relying on the C# 10 [interpolated string handlers](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler) feature.

But unlike classical usage of interpolated string handlers, a FixedString is in fact itself **an interpolation handler**, using a fixed size buffer to store the string.

| Fixed String Type | Maximum Number of Chars | Total Size
|-------------------|-------------------------|---------------------
| `FixedString8`    |        7                | 16 bytes
| `FixedString16`   |        15               | 32 bytes
| `FixedString32`   |        31               | 64 bytes
| `FixedString64`   |        63               | 128 bytes

Each fixed string contains a `short` value for the length.

For example `FixedString8` is declared like this:

```c#
[InterpolatedStringHandler]
public struct FixedString8 : IFixedString<FixedString8>
{
    private short _length;
    private short _c0;
    private short _c1;
    private short _c2;
    private short _c3;
    private short _c4;
    private short _c5;
    private short _c6;

    // ...
}
```

It is using a `short` for the internal representation of each characters to avoid marshalling issues - so that each character is passed as a plain `short` (UTF-16) value to the native side.

You can then copy around by value this string very easily and you can have a better control of the layout in memory.

### Usage and restrictions

FixedStrings are not meant to replace all strings in your application. They are meant to be used for strings that are known to be short and that are used in a very hot path of your application.

- If you try to add more characters than the size of the fixed string, **it will be truncated**. For practical reasons, It won't throw an exception!
- FixedStrings support interpolation:
  ```c#
  FixedString8 str = $"Hello {name}";
  ```
  For performance reasons, supported types for values are: `string` and any types implementing `ISpanFormattable` (e.g `int`, `float`...etc.).
  A fixed string is itself `ISpanFormattable` and can be used as a value - and nested in string interpolations.

  > Note that if a non string interpolated value cannot fit within the fixed string, it won't be emitted and the string will be truncated (in case of using a string). For example 
  > ```c#
  > FixedString8 str = $"{"Hello"}{"World"}";
  > ```
  > will result in ` str` containing `"HelloWo"` (7 characters).
  > but the following interpolation:
  > ```c#
  > FixedString8 str = $"Hello{int.MaxValue}";
  > ```
  > will result in ` str` containing `"Hello"`, with the int value being discarded as it cannot fit within the fixed string.
- Supporting alignment and format for interpolated values
  ```c#
  byte value = 10; 
  FixedString16 str = $"Test 0x{value:X2}";
  ```
- FixedStrings support assigning from a regular string:
  ```c#
  FixedString8 str = "Hello";
  ```
- You can get a `ReadOnlySpan<char>` directly from a FixedString:
  ```c#
  FixedString8 str = "Hello";
  ReadOnlySpan<char> span = str.AsSpan();
  ```
- FixedStrings can be copied by value:
  ```c#
  FixedString8 str = "Hello";
  FixedString8 str2 = str;
  ```
  `str2` will be a copy of `str`.

- FixedStrings implement [`IFixedString<T>`](https://github.com/xoofx/FixedStrings/blob/main/src/FixedStrings/IFixedString.cs) and can be used as a generic constraint:
  ```c#
  public int Foo<T>(T value) where T : IFixedString<T>
  {
      // ...
      return value.Length;
  }
  ```

## Benchmarks

`TestFixed` is using a `FixedString` and `TestDynamic` is using a regular `string`.

- For `FixedString8`, the performance gain is 2.5x faster than using a regular string.
- For `FixedString16`, the performance gain is 2x faster than using a regular string.
- For `FixedString32` / `FixedString64`, the performance gain is 30% faster than using a regular string.

| Method        | Runtime  | Mean     | Allocated |
|-------------- |--------- |---------:|----------:|
| TestFixed8    | .NET 7.0 | 13.70 ns |         - |
| TestDynamic8  | .NET 7.0 | 31.33 ns |      40 B |
|               |          |          |           |
| TestFixed8    | .NET 8.0 | 10.05 ns |         - |
| TestDynamic8  | .NET 8.0 | 26.47 ns |      40 B |
|               |          |          |           |
| TestFixed16   | .NET 7.0 | 17.56 ns |         - |
| TestDynamic16 | .NET 7.0 | 32.52 ns |      56 B |
|               |          |          |           |
| TestFixed16   | .NET 8.0 | 14.25 ns |         - |
| TestDynamic16 | .NET 8.0 | 27.80 ns |      56 B |
|               |          |          |           |
| TestFixed32   | .NET 7.0 | 38.27 ns |         - |
| TestDynamic32 | .NET 7.0 | 49.70 ns |      86 B |
|               |          |          |           |
| TestFixed32   | .NET 8.0 | 30.12 ns |         - |
| TestDynamic32 | .NET 8.0 | 38.15 ns |      86 B |
|               |          |          |           |
| TestFixed64   | .NET 7.0 | 61.37 ns |         - |
| TestDynamic64 | .NET 7.0 | 80.15 ns |     144 B |
|               |          |          |           |
| TestFixed64   | .NET 8.0 | 43.96 ns |         - |
| TestDynamic64 | .NET 8.0 | 59.32 ns |     144 B |

## Building FixedStrings

In order to build FixedStrings, you need:
- Install the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Install the [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) (For the benchmarks)

```
dotnet build -c Release src/FixedStrings.sln
```

## License

This software is released under the [BSD-2-Clause license](https://opensource.org/licenses/BSD-2-Clause). 

## Author

Alexandre Mutel aka [xoofx](https://xoofx.github.io).
