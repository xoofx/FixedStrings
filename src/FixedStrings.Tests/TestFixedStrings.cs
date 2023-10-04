// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NUnit.Framework;

namespace FixedStrings.Tests;

public class TestFixedStrings
{
    [Test]
    public void TestHelloWorld()
    {
        FixedString16 str = $"HelloWorld {DateTime.Now.Year}";
        Console.Out.WriteLine(str.AsSpan());
        Assert.AreEqual($"HelloWorld {DateTime.Now.Year}", str.ToString());
    }

    [Test]
    public void TestReadmeHexadecimal()
    {
        byte value = 10;
        FixedString16 str = $"Test 0x{value:X2}";
        Assert.AreEqual($"Test 0x{value:X2}", str.ToString());
    }

    [TestCase("")]
    [TestCase("0")]
    [TestCase("01")]
    [TestCase("01234567")]
    [TestCase("0123456789")]
    [TestCase("01234567890123456789")]
    [TestCase("012345678901234567890123456789")]
    [TestCase("0123456789012345678901234567890123456789")]
    public void Literal8(string test)
    {
        FixedString8 fs = test;
        AssertFixedString(fs, test);
    }

    [TestCase("")]
    [TestCase("0")]
    [TestCase("01")]
    [TestCase("01234567")]
    [TestCase("0123456789")]
    [TestCase("01234567890123456789")]
    [TestCase("012345678901234567890123456789")]
    [TestCase("0123456789012345678901234567890123456789")]
    public void Literal16(string test)
    {
        FixedString16 fs = test;
        AssertFixedString(fs, test);
    }

    [TestCase("")]
    [TestCase("0")]
    [TestCase("01")]
    [TestCase("01234567")]
    [TestCase("0123456789")]
    [TestCase("01234567890123456789")]
    [TestCase("012345678901234567890123456789")]
    [TestCase("0123456789012345678901234567890123456789")]
    public void Literal32(string test)
    {
        FixedString32 fs = test;
        AssertFixedString(fs, test);
    }

    [TestCase("")]
    [TestCase("0")]
    [TestCase("01")]
    [TestCase("01234567")]
    [TestCase("0123456789")]
    [TestCase("01234567890123456789")]
    [TestCase("012345678901234567890123456789")]
    [TestCase("0123456789012345678901234567890123456789")]
    [TestCase("012345678901234567890123456789012345678901234567890123456789")]
    [TestCase("0123456789012345678901234567890123456789012345678901234567890123456789")]
    public void Literal64(string test)
    {
        FixedString64 fs = test;
        AssertFixedString(fs, test);
    }

    [TestCase(0, "Hello 0")]
    [TestCase(12, "Hello ")]
    public void Interpolated8(int value, string expected)
    {
        FixedString8 fs = $"Hello {value}";
        AssertFixedString(fs, expected);
    }

    [TestCase(0, "Hello 0")]
    [TestCase(12, "Hello 12")]
    [TestCase(123456789, "Hello 123456789")]
    [TestCase(1234567890, "Hello ")]
    public void Interpolated16(int value, string expected)
    {
        FixedString16 fs = $"Hello {value}";
        AssertFixedString(fs, expected);
    }

    [TestCase(0, "Hello1234567890=0")]
    [TestCase(12, "Hello1234567890=12")]
    [TestCase(1234567890, "Hello1234567890=1234567890")]
    public void Interpolated32(int value, string expected)
    {
        FixedString32 fs = $"Hello1234567890={value}";
        AssertFixedString(fs, expected);
    }

    [TestCase(0, "Hello1234567890=0")]
    [TestCase(12, "Hello1234567890=12")]
    [TestCase(1234567890, "Hello1234567890=1234567890")]
    public void Interpolated64(int value, string expected)
    {
        FixedString64 fs = $"Hello1234567890={value}";
        AssertFixedString(fs, expected);
    }

    [Test]
    public void AppendLiteralOverflow()
    {
        // Check overflow on AddLiteral
        {
            var fullString = new string('A', FixedString8.MaxLength);
            FixedString8 fsFixed = $"{fullString}{fullString}";
            Assert.AreEqual(fullString, fsFixed.ToString());
        }
        {
            var fullString = new string('A', FixedString16.MaxLength);
            FixedString16 fsFixed = $"{fullString}{fullString}";
            Assert.AreEqual(fullString, fsFixed.ToString());
        }
        {
            var fullString = new string('A', FixedString32.MaxLength);
            FixedString32 fsFixed = $"{fullString}{fullString}";
            Assert.AreEqual(fullString, fsFixed.ToString());
        }
        {
            var fullString = new string('A', FixedString64.MaxLength);
            FixedString64 fsFixed = $"{fullString}{fullString}";
            Assert.AreEqual(fullString, fsFixed.ToString());
        }
    }

    [Test]
    public void TestAlignmentAndFormatFixedString8()
    {
        FixedString8 test;
        const int maxLength = 7;
        const string str = "Hello"; // Length = 5

        {
            // AppendFormatted
            int value = 10;
            test = $"{value,3}";
            Assert.AreEqual($"{value,3}", test.ToString());

            test = $"{value,3:X}";
            Assert.AreEqual($"{value,3:X}", test.ToString());

            test = $"{value:X}";
            Assert.AreEqual($"{value:X}", test.ToString());
        }

        test = $"{str,-5}";
        Assert.AreEqual($"{str,-5}", test.ToString());

        test = $"{str,0}";
        Assert.AreEqual($"{str,0}", test.ToString());

        test = $"{str,-7}";
        Assert.AreEqual($"{str,-7}"[..maxLength], test.ToString());

        test = $"{str,-8}";
        Assert.AreEqual($"{str,-8}"[..maxLength], test.ToString());

        test = $"{str,7}";
        Assert.AreEqual($"{str,7}"[..maxLength], test.ToString());

        test = $"{str,11}";
        Assert.AreEqual($"{str,11}"[..maxLength], test.ToString());

        test = $"{str,12}";
        Assert.AreEqual($"{str,12}"[..maxLength], test.ToString());

        test = $"{str,13}";
        Assert.AreEqual($"{str,13}"[..maxLength], test.ToString());
    }

    [Test]
    public void TestAlignmentAndFormatFixedString16()
    {
        FixedString16 test;
        const int maxLength = 15;
        const string str = "Hello01234567"; // Length = 13

        {
            // AppendFormatted
            int value = 10;
            test = $"{value,3}";
            Assert.AreEqual($"{value,3}", test.ToString());

            test = $"{value,3:X}";
            Assert.AreEqual($"{value,3:X}", test.ToString());

            test = $"{value:X}";
            Assert.AreEqual($"{value:X}", test.ToString());
        }

        test = $"{str,-13}";
        Assert.AreEqual($"{str,-13}", test.ToString());

        test = $"{str,0}";
        Assert.AreEqual($"{str,0}", test.ToString());

        test = $"{str,-15}";
        Assert.AreEqual($"{str,-15}"[..maxLength], test.ToString());

        test = $"{str,-16}";
        Assert.AreEqual($"{str,-16}"[..maxLength], test.ToString());

        test = $"{str,15}";
        Assert.AreEqual($"{str,15}"[..maxLength], test.ToString());

        test = $"{str,19}";
        Assert.AreEqual($"{str,19}"[..maxLength], test.ToString());

        test = $"{str,20}";
        Assert.AreEqual($"{str,20}"[..maxLength], test.ToString());

        test = $"{str,21}";
        Assert.AreEqual($"{str,21}"[..maxLength], test.ToString());
    }

    [Test]
    public void TestAlignmentAndFormatFixedString32()
    {
        FixedString32 test;
        const int maxLength = 31;
        const string str = "Hello012345678901234567890123"; // Length = 29

        {
            // AppendFormatted
            int value = 10;
            test = $"{value,3}";
            Assert.AreEqual($"{value,3}", test.ToString());

            test = $"{value,3:X}";
            Assert.AreEqual($"{value,3:X}", test.ToString());

            test = $"{value:X}";
            Assert.AreEqual($"{value:X}", test.ToString());
        }

        test = $"{str,-29}";
        Assert.AreEqual($"{str,-29}", test.ToString());

        test = $"{str,0}";
        Assert.AreEqual($"{str,0}", test.ToString());

        test = $"{str,-31}";
        Assert.AreEqual($"{str,-31}"[..maxLength], test.ToString());

        test = $"{str,-32}";
        Assert.AreEqual($"{str,-32}"[..maxLength], test.ToString());

        test = $"{str,31}";
        Assert.AreEqual($"{str,31}"[..maxLength], test.ToString());

        test = $"{str,35}";
        Assert.AreEqual($"{str,35}"[..maxLength], test.ToString());

        test = $"{str,36}";
        Assert.AreEqual($"{str,36}"[..maxLength], test.ToString());

        test = $"{str,37}";
        Assert.AreEqual($"{str,37}"[..maxLength], test.ToString());
    }

    [Test]
    public void TestAlignmentAndFormatFixedString64()
    {
        FixedString64 test;
        const int maxLength = 63;
        const string str = "Hello01234567890123456789012345678901234567890123456789012345"; // Length = 61

        {
            // AppendFormatted
            int value = 10;
            test = $"{value,3}";
            Assert.AreEqual($"{value,3}", test.ToString());

            test = $"{value,3:X}";
            Assert.AreEqual($"{value,3:X}", test.ToString());

            test = $"{value:X}";
            Assert.AreEqual($"{value:X}", test.ToString());
        }

        test = $"{str,-61}";
        Assert.AreEqual($"{str,-61}", test.ToString());

        test = $"{str,0}";
        Assert.AreEqual($"{str,0}", test.ToString());

        test = $"{str,-63}";
        Assert.AreEqual($"{str,-63}"[..maxLength], test.ToString());

        test = $"{str,-64}";
        Assert.AreEqual($"{str,-64}"[..maxLength], test.ToString());

        test = $"{str,63}";
        Assert.AreEqual($"{str,63}"[..maxLength], test.ToString());

        test = $"{str,64}";
        Assert.AreEqual($"{str,64}"[..maxLength], test.ToString());

        test = $"{str,65}";
        Assert.AreEqual($"{str,65}"[..maxLength], test.ToString());

        test = $"{str,66}";
        Assert.AreEqual($"{str,66}"[..maxLength], test.ToString());
    }

    private static unsafe void AssertFixedString<TFixedString>(TFixedString fs, string test) where TFixedString: unmanaged, IFixedString<TFixedString>
    {
        // Verify the size of the fixed struct
        Assert.AreEqual((TFixedString.MaxLength + 1) * sizeof(short), sizeof(TFixedString));

        // Verify ToString();
        var expectedLength = Math.Min(test.Length, TFixedString.MaxLength);
        var expectedString = test.Substring(0, expectedLength);
        Assert.AreEqual(expectedString, fs.ToString());

        // Verify Length
        Assert.AreEqual(expectedLength, fs.Length);

        // Verify GetHashCode
        Assert.NotNull(fs.GetHashCode());

        // Verify AsSpan()
        Assert.True(expectedString.AsSpan().SequenceEqual(fs.AsSpan()));

        // Verify TryFormat()
        var tryFormat = $"{fs}";
        Assert.AreEqual(expectedString, tryFormat);

        // Test Equals and implicit operator
        TFixedString fsCopy = expectedString;
        Assert.AreEqual(fsCopy, fs);
        Assert.True(fsCopy.Equals((object)fs));
        Assert.False(fsCopy.Equals((object)1));

        // Verify Clear()
        fs.Clear();
        Assert.AreEqual(0, fs.Length);

        TFixedString tooBig = "ABCD";
        Span<char> span = stackalloc char[3];
        Assert.False(tooBig.TryFormat(span, out var charsWritten, ReadOnlySpan<char>.Empty, null));
    }
}
