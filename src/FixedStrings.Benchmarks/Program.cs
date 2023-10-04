// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace FixedStrings.Benchmarks;

[MemoryDiagnoser]
public class BenchString
{
    private readonly Random _random = new Random(0);

    [Benchmark]
    public FixedString8 TestFixed8()
    {
        return $"Hello{_random.Next(100)}";
    }

    [Benchmark]
    public string TestDynamic8()
    {
        return $"Hello{_random.Next(100)}";
    }

    [Benchmark]
    public FixedString16 TestFixed16()
    {
        return $"Hello {_random.Next(100)} World!";
    }

    [Benchmark]
    public string TestDynamic16()
    {
        return $"Hello {_random.Next(100)} World!";
    }

    [Benchmark]
    public FixedString32 TestFixed32()
    {
        return $"Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!";
    }

    [Benchmark]
    public string TestDynamic32()
    {
        return $"Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!";
    }

    [Benchmark]
    public FixedString32 TestFixed64()
    {
        return $"Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!";
    }

    [Benchmark]
    public string TestDynamic64()
    {
        return $"Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!Hello {_random.Next(100000)} World {_random.Next(100000)} Multi!";
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<BenchString>(null, args);
    }
}