// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace FixedStrings;

/// <summary>
/// The base interface for a fixed string.
/// </summary>
public interface IFixedString : ISpanFormattable
{
    /// <summary>
    /// Gets the maximum number of characters in this fixed string.
    /// </summary>
    static abstract int MaxLength { get; }

    /// <summary>
    /// Gets the number of characters in the string. The length is always less than or equal to <see cref="MaxLength"/>.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Resets this string to an empty string.
    /// </summary>
    void Clear();

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    ReadOnlySpan<char> AsSpan();
}

/// <summary>
/// The base interface for a fixed string with its implementation type.
/// </summary>
public interface IFixedString<T> : IFixedString, IEquatable<T> where T: IFixedString<T>
{
    /// <summary>
    /// Converts a string to a fixed string.
    /// </summary>
    /// <param name="s">The string to convert to a fixed string.</param>
    static abstract implicit operator T(string s);
}
