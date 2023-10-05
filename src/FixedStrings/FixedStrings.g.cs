// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable

namespace FixedStrings;

/// <summary>
/// Represents a fixed-length string of maximum 7 characters.
/// </summary>
[InterpolatedStringHandler]
public struct FixedString8 : IFixedString<FixedString8>
{
    /// <inheritdoc cref="IFixedString{T}.MaxLength"/>
    public static int MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 7;
    }

    private short _length;
#pragma warning disable CS0169 // Field is never used
    // Use short instead of char to make this struct blittable
    private short _c0;
    private short _c1;
    private short _c2;
    private short _c3;
    private short _c4;
    private short _c5;
    private short _c6;
#pragma warning restore CS0169 // Field is never used

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString8"/> struct.
    /// </summary>
    public FixedString8(int literalLength, int formattedCount)
    {
        _length = 0;
    }

        /// <summary>
    /// Initializes a new instance of the <see cref="FixedString8"/> struct.
    /// </summary>
    public FixedString8(string value)
    {
        _length = 0;
        AppendLiteral(value);
    }

    /// <inheriteddoc cref="IFixedString{T}.Length"/>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <inheriteddoc cref="IFixedString{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _length = 0;

    /// <summary>
    /// Appends the string literal to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        if (_length == MaxLength) return;

        var span = AsRemainingSpan();
        if (span.Length < s.Length)
        {
            s.Slice(0, span.Length).CopyTo(span);
            _length += (short)span.Length;
        }
        else
        {
            s.CopyTo(span);
            _length += (short)s.Length;
        }
    }

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t) => AppendLiteral(t);

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t, int alignment)
    {
        var startPosition = _length;
        AppendFormatted(t);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T t) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        t.TryFormat(span, out int charsWritten, new ReadOnlySpan<char>(), null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        value.TryFormat(span, out int charsWritten, format, null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value, format);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FixedString8 other) => _length == other._length && AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritedoc />
    public override bool Equals(object? obj) => obj is FixedString8 other && Equals(other);

    /// <inheritedoc />
    public override int GetHashCode()
    {
        // Compute the FNV-1a hash of the string
        int hash = unchecked((int)2166136261);
        foreach (var c in AsSpan())
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public readonly ReadOnlySpan<char> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref Unsafe.AsRef(_c0)), _length);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Span<char> IFixedString.GetUnsafeFullSpan() => AsUnsafeFullSpan();

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="FixedString8"/>.
    [SkipLocalsInit]    
    public static implicit operator FixedString8(string s) => new(s);

    /// <inheritedoc />
    public readonly override string ToString() => ToString(null, null);

    /// <inheritedoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _length == 0 ? string.Empty : new(AsSpan());

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < _length)
        {
            charsWritten = 0;
            return false;
        }
        AsSpan().CopyTo(destination);
        charsWritten = _length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> AsUnsafeFullSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref _c0), MaxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    private Span<char> AsRemainingSpan() => AsUnsafeFullSpan().Slice(_length);

    /// <summary>
    /// Appends or inserts the specified alignment at the specified position.
    /// </summary>
    private void AppendOrInsertAlignment(int startPosition, int alignment)
    {
        if (alignment == 0) return;

        int length = _length - startPosition;
        bool padAfter = false;
        if (alignment < 0)
        {
            padAfter = true;
            alignment = -alignment;
        }

        int numberOfCharsToAppendOrInsert = alignment - length;
        if (numberOfCharsToAppendOrInsert <= 0) return;

        if (padAfter)
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, _length + numberOfCharsToAppendOrInsert) - _length;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                AsRemainingSpan().Slice(0, numberOfCharsToAppendOrInsert).Fill(' ');
                _length += (short)numberOfCharsToAppendOrInsert;
            }
        }
        else 
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, startPosition + numberOfCharsToAppendOrInsert) - startPosition;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                var endPositionFill = startPosition + numberOfCharsToAppendOrInsert;

                var span = AsUnsafeFullSpan();
                if (endPositionFill < MaxLength)
                {
                    var maxLengthToCopy = Math.Min(MaxLength, endPositionFill + length) - endPositionFill;
                    if (maxLengthToCopy > 0)
                    {
                        span.Slice(startPosition, maxLengthToCopy).CopyTo(span.Slice(endPositionFill, maxLengthToCopy));
                    }
                }

                span.Slice(startPosition, numberOfCharsToAppendOrInsert).Fill(' ');
                _length = (short)Math.Min(endPositionFill + length, MaxLength);
            }
        }
    }
}

/// <summary>
/// Represents a fixed-length string of maximum 15 characters.
/// </summary>
[InterpolatedStringHandler]
public struct FixedString16 : IFixedString<FixedString16>
{
    /// <inheritdoc cref="IFixedString{T}.MaxLength"/>
    public static int MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 15;
    }

    private short _length;
#pragma warning disable CS0169 // Field is never used
    // Use short instead of char to make this struct blittable
    private short _c0;
    private short _c1;
    private short _c2;
    private short _c3;
    private short _c4;
    private short _c5;
    private short _c6;
    private short _c7;
    private short _c8;
    private short _c9;
    private short _c10;
    private short _c11;
    private short _c12;
    private short _c13;
    private short _c14;
#pragma warning restore CS0169 // Field is never used

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString16"/> struct.
    /// </summary>
    public FixedString16(int literalLength, int formattedCount)
    {
        _length = 0;
    }

        /// <summary>
    /// Initializes a new instance of the <see cref="FixedString16"/> struct.
    /// </summary>
    public FixedString16(string value)
    {
        _length = 0;
        AppendLiteral(value);
    }

    /// <inheriteddoc cref="IFixedString{T}.Length"/>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <inheriteddoc cref="IFixedString{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _length = 0;

    /// <summary>
    /// Appends the string literal to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        if (_length == MaxLength) return;

        var span = AsRemainingSpan();
        if (span.Length < s.Length)
        {
            s.Slice(0, span.Length).CopyTo(span);
            _length += (short)span.Length;
        }
        else
        {
            s.CopyTo(span);
            _length += (short)s.Length;
        }
    }

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t) => AppendLiteral(t);

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t, int alignment)
    {
        var startPosition = _length;
        AppendFormatted(t);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T t) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        t.TryFormat(span, out int charsWritten, new ReadOnlySpan<char>(), null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        value.TryFormat(span, out int charsWritten, format, null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value, format);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FixedString16 other) => _length == other._length && AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritedoc />
    public override bool Equals(object? obj) => obj is FixedString16 other && Equals(other);

    /// <inheritedoc />
    public override int GetHashCode()
    {
        // Compute the FNV-1a hash of the string
        int hash = unchecked((int)2166136261);
        foreach (var c in AsSpan())
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public readonly ReadOnlySpan<char> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref Unsafe.AsRef(_c0)), _length);

    Span<char> IFixedString.GetUnsafeFullSpan() => AsUnsafeFullSpan();

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="FixedString16"/>.
    [SkipLocalsInit]    
    public static implicit operator FixedString16(string s) => new(s);

    /// <inheritedoc />
    public readonly override string ToString() => ToString(null, null);

    /// <inheritedoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _length == 0 ? string.Empty : new(AsSpan());

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < _length)
        {
            charsWritten = 0;
            return false;
        }
        AsSpan().CopyTo(destination);
        charsWritten = _length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> AsUnsafeFullSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref _c0), MaxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    private Span<char> AsRemainingSpan() => AsUnsafeFullSpan().Slice(_length);

    /// <summary>
    /// Appends or inserts the specified alignment at the specified position.
    /// </summary>
    private void AppendOrInsertAlignment(int startPosition, int alignment)
    {
        if (alignment == 0) return;

        int length = _length - startPosition;
        bool padAfter = false;
        if (alignment < 0)
        {
            padAfter = true;
            alignment = -alignment;
        }

        int numberOfCharsToAppendOrInsert = alignment - length;
        if (numberOfCharsToAppendOrInsert <= 0) return;

        if (padAfter)
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, _length + numberOfCharsToAppendOrInsert) - _length;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                AsRemainingSpan().Slice(0, numberOfCharsToAppendOrInsert).Fill(' ');
                _length += (short)numberOfCharsToAppendOrInsert;
            }
        }
        else 
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, startPosition + numberOfCharsToAppendOrInsert) - startPosition;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                var endPositionFill = startPosition + numberOfCharsToAppendOrInsert;

                var span = AsUnsafeFullSpan();
                if (endPositionFill < MaxLength)
                {
                    var maxLengthToCopy = Math.Min(MaxLength, endPositionFill + length) - endPositionFill;
                    if (maxLengthToCopy > 0)
                    {
                        span.Slice(startPosition, maxLengthToCopy).CopyTo(span.Slice(endPositionFill, maxLengthToCopy));
                    }
                }

                span.Slice(startPosition, numberOfCharsToAppendOrInsert).Fill(' ');
                _length = (short)Math.Min(endPositionFill + length, MaxLength);
            }
        }
    }
}

/// <summary>
/// Represents a fixed-length string of maximum 31 characters.
/// </summary>
[InterpolatedStringHandler]
public struct FixedString32 : IFixedString<FixedString32>
{
    /// <inheritdoc cref="IFixedString{T}.MaxLength"/>
    public static int MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 31;
    }

    private short _length;
#pragma warning disable CS0169 // Field is never used
    // Use short instead of char to make this struct blittable
    private short _c0;
    private short _c1;
    private short _c2;
    private short _c3;
    private short _c4;
    private short _c5;
    private short _c6;
    private short _c7;
    private short _c8;
    private short _c9;
    private short _c10;
    private short _c11;
    private short _c12;
    private short _c13;
    private short _c14;
    private short _c15;
    private short _c16;
    private short _c17;
    private short _c18;
    private short _c19;
    private short _c20;
    private short _c21;
    private short _c22;
    private short _c23;
    private short _c24;
    private short _c25;
    private short _c26;
    private short _c27;
    private short _c28;
    private short _c29;
    private short _c30;
#pragma warning restore CS0169 // Field is never used

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString32"/> struct.
    /// </summary>
    public FixedString32(int literalLength, int formattedCount)
    {
        _length = 0;
    }

        /// <summary>
    /// Initializes a new instance of the <see cref="FixedString32"/> struct.
    /// </summary>
    public FixedString32(string value)
    {
        _length = 0;
        AppendLiteral(value);
    }

    /// <inheriteddoc cref="IFixedString{T}.Length"/>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <inheriteddoc cref="IFixedString{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _length = 0;

    /// <summary>
    /// Appends the string literal to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        if (_length == MaxLength) return;

        var span = AsRemainingSpan();
        if (span.Length < s.Length)
        {
            s.Slice(0, span.Length).CopyTo(span);
            _length += (short)span.Length;
        }
        else
        {
            s.CopyTo(span);
            _length += (short)s.Length;
        }
    }

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t) => AppendLiteral(t);

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t, int alignment)
    {
        var startPosition = _length;
        AppendFormatted(t);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T t) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        t.TryFormat(span, out int charsWritten, new ReadOnlySpan<char>(), null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        value.TryFormat(span, out int charsWritten, format, null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value, format);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FixedString32 other) => _length == other._length && AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritedoc />
    public override bool Equals(object? obj) => obj is FixedString32 other && Equals(other);

    /// <inheritedoc />
    public override int GetHashCode()
    {
        // Compute the FNV-1a hash of the string
        int hash = unchecked((int)2166136261);
        foreach (var c in AsSpan())
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public readonly ReadOnlySpan<char> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref Unsafe.AsRef(_c0)), _length);

    Span<char> IFixedString.GetUnsafeFullSpan() => AsUnsafeFullSpan();

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="FixedString32"/>.
    [SkipLocalsInit]    
    public static implicit operator FixedString32(string s) => new(s);

    /// <inheritedoc />
    public readonly override string ToString() => ToString(null, null);

    /// <inheritedoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _length == 0 ? string.Empty : new(AsSpan());

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < _length)
        {
            charsWritten = 0;
            return false;
        }
        AsSpan().CopyTo(destination);
        charsWritten = _length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> AsUnsafeFullSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref _c0), MaxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    private Span<char> AsRemainingSpan() => AsUnsafeFullSpan().Slice(_length);

    /// <summary>
    /// Appends or inserts the specified alignment at the specified position.
    /// </summary>
    private void AppendOrInsertAlignment(int startPosition, int alignment)
    {
        if (alignment == 0) return;

        int length = _length - startPosition;
        bool padAfter = false;
        if (alignment < 0)
        {
            padAfter = true;
            alignment = -alignment;
        }

        int numberOfCharsToAppendOrInsert = alignment - length;
        if (numberOfCharsToAppendOrInsert <= 0) return;

        if (padAfter)
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, _length + numberOfCharsToAppendOrInsert) - _length;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                AsRemainingSpan().Slice(0, numberOfCharsToAppendOrInsert).Fill(' ');
                _length += (short)numberOfCharsToAppendOrInsert;
            }
        }
        else 
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, startPosition + numberOfCharsToAppendOrInsert) - startPosition;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                var endPositionFill = startPosition + numberOfCharsToAppendOrInsert;

                var span = AsUnsafeFullSpan();
                if (endPositionFill < MaxLength)
                {
                    var maxLengthToCopy = Math.Min(MaxLength, endPositionFill + length) - endPositionFill;
                    if (maxLengthToCopy > 0)
                    {
                        span.Slice(startPosition, maxLengthToCopy).CopyTo(span.Slice(endPositionFill, maxLengthToCopy));
                    }
                }

                span.Slice(startPosition, numberOfCharsToAppendOrInsert).Fill(' ');
                _length = (short)Math.Min(endPositionFill + length, MaxLength);
            }
        }
    }
}

/// <summary>
/// Represents a fixed-length string of maximum 63 characters.
/// </summary>
[InterpolatedStringHandler]
public struct FixedString64 : IFixedString<FixedString64>
{
    /// <inheritdoc cref="IFixedString{T}.MaxLength"/>
    public static int MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 63;
    }

    private short _length;
#pragma warning disable CS0169 // Field is never used
    // Use short instead of char to make this struct blittable
    private short _c0;
    private short _c1;
    private short _c2;
    private short _c3;
    private short _c4;
    private short _c5;
    private short _c6;
    private short _c7;
    private short _c8;
    private short _c9;
    private short _c10;
    private short _c11;
    private short _c12;
    private short _c13;
    private short _c14;
    private short _c15;
    private short _c16;
    private short _c17;
    private short _c18;
    private short _c19;
    private short _c20;
    private short _c21;
    private short _c22;
    private short _c23;
    private short _c24;
    private short _c25;
    private short _c26;
    private short _c27;
    private short _c28;
    private short _c29;
    private short _c30;
    private short _c31;
    private short _c32;
    private short _c33;
    private short _c34;
    private short _c35;
    private short _c36;
    private short _c37;
    private short _c38;
    private short _c39;
    private short _c40;
    private short _c41;
    private short _c42;
    private short _c43;
    private short _c44;
    private short _c45;
    private short _c46;
    private short _c47;
    private short _c48;
    private short _c49;
    private short _c50;
    private short _c51;
    private short _c52;
    private short _c53;
    private short _c54;
    private short _c55;
    private short _c56;
    private short _c57;
    private short _c58;
    private short _c59;
    private short _c60;
    private short _c61;
    private short _c62;
#pragma warning restore CS0169 // Field is never used

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString64"/> struct.
    /// </summary>
    public FixedString64(int literalLength, int formattedCount)
    {
        _length = 0;
    }

        /// <summary>
    /// Initializes a new instance of the <see cref="FixedString64"/> struct.
    /// </summary>
    public FixedString64(string value)
    {
        _length = 0;
        AppendLiteral(value);
    }

    /// <inheriteddoc cref="IFixedString{T}.Length"/>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <inheriteddoc cref="IFixedString{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _length = 0;

    /// <summary>
    /// Appends the string literal to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        if (_length == MaxLength) return;

        var span = AsRemainingSpan();
        if (span.Length < s.Length)
        {
            s.Slice(0, span.Length).CopyTo(span);
            _length += (short)span.Length;
        }
        else
        {
            s.CopyTo(span);
            _length += (short)s.Length;
        }
    }

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t) => AppendLiteral(t);

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t, int alignment)
    {
        var startPosition = _length;
        AppendFormatted(t);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T t) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        t.TryFormat(span, out int charsWritten, new ReadOnlySpan<char>(), null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) where T : ISpanFormattable
    {
        var span = AsRemainingSpan();
        value.TryFormat(span, out int charsWritten, format, null);
        _length += (short)charsWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : ISpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value, format);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FixedString64 other) => _length == other._length && AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritedoc />
    public override bool Equals(object? obj) => obj is FixedString64 other && Equals(other);

    /// <inheritedoc />
    public override int GetHashCode()
    {
        // Compute the FNV-1a hash of the string
        int hash = unchecked((int)2166136261);
        foreach (var c in AsSpan())
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public readonly ReadOnlySpan<char> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref Unsafe.AsRef(_c0)), _length);

    Span<char> IFixedString.GetUnsafeFullSpan() => AsUnsafeFullSpan();

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="FixedString64"/>.
    [SkipLocalsInit]    
    public static implicit operator FixedString64(string s) => new(s);

    /// <inheritedoc />
    public readonly override string ToString() => ToString(null, null);

    /// <inheritedoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _length == 0 ? string.Empty : new(AsSpan());

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < _length)
        {
            charsWritten = 0;
            return false;
        }
        AsSpan().CopyTo(destination);
        charsWritten = _length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> AsUnsafeFullSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<short, char>(ref _c0), MaxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    private Span<char> AsRemainingSpan() => AsUnsafeFullSpan().Slice(_length);

    /// <summary>
    /// Appends or inserts the specified alignment at the specified position.
    /// </summary>
    private void AppendOrInsertAlignment(int startPosition, int alignment)
    {
        if (alignment == 0) return;

        int length = _length - startPosition;
        bool padAfter = false;
        if (alignment < 0)
        {
            padAfter = true;
            alignment = -alignment;
        }

        int numberOfCharsToAppendOrInsert = alignment - length;
        if (numberOfCharsToAppendOrInsert <= 0) return;

        if (padAfter)
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, _length + numberOfCharsToAppendOrInsert) - _length;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                AsRemainingSpan().Slice(0, numberOfCharsToAppendOrInsert).Fill(' ');
                _length += (short)numberOfCharsToAppendOrInsert;
            }
        }
        else 
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, startPosition + numberOfCharsToAppendOrInsert) - startPosition;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                var endPositionFill = startPosition + numberOfCharsToAppendOrInsert;

                var span = AsUnsafeFullSpan();
                if (endPositionFill < MaxLength)
                {
                    var maxLengthToCopy = Math.Min(MaxLength, endPositionFill + length) - endPositionFill;
                    if (maxLengthToCopy > 0)
                    {
                        span.Slice(startPosition, maxLengthToCopy).CopyTo(span.Slice(endPositionFill, maxLengthToCopy));
                    }
                }

                span.Slice(startPosition, numberOfCharsToAppendOrInsert).Fill(' ');
                _length = (short)Math.Min(endPositionFill + length, MaxLength);
            }
        }
    }
}

