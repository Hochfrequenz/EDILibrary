// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;

namespace EDILibrary
{
    public static class StringExtensions
    {

        // the string.Split() method from .NET tend to run out of memory on 80 Mb strings. 
        // this has been reported several places online. 
        // This version is fast and memory efficient and return no empty lines. 
        public static List<string> LowMemSplit(this string s, string seperator)
        {
            var list = new List<string>();
            var lastPos = 0;
            var pos = s.IndexOf(seperator); // warn: string.IndexOf(string) is culture-specific
            while (pos > -1)
            {
                while (pos == lastPos)
                {
                    lastPos += seperator.Length;
                    pos = s.IndexOf(seperator, lastPos); // warn: string.IndexOf(string) is culture-specific
                    if (pos == -1)
                        return list;
                }

                var tmp = s.Substring(lastPos, pos - lastPos);
                if (tmp.Trim().Length > 0)
                    list.Add(tmp);
                lastPos = pos + seperator.Length;
                pos = s.IndexOf(seperator, lastPos); // warn: string.IndexOf(string) is culture-specific
            }

            if (lastPos < s.Length)
            {
                var tmp = s.Substring(lastPos, s.Length - lastPos);
                if (tmp.Trim().Length > 0)
                    list.Add(tmp);
            }

            return list;
        }

        public static LineSplitEnumerator SplitLines(this string str, char separator)
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new LineSplitEnumerator(str.AsSpan(), separator);
        }

        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;
            private char separator;
            public LineSplitEnumerator(ReadOnlySpan<char> str, char separator)
            {
                _str = str;
                Current = default;
                this.separator = separator;
            }

            // Needed to be compatible with the foreach operator
            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOf(separator);
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                    Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                    return true;
                }

                //if (index < span.Length - 1 && span[index] == '\r')
                //{
                //    // Try to consume the '\n' associated to the '\r'
                //    var next = span[index + 1];
                //    if (next == '\n')
                //    {
                //        Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
                //        _str = span.Slice(index + 2);
                //        return true;
                //    }
                //}

                Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
                _str = span.Slice(index + 1);
                return true;
            }

            public LineSplitEntry Current { get; private set; }
        }

        public readonly ref struct LineSplitEntry
        {
            public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
            {
                Line = line;
                Separator = separator;
            }

            public ReadOnlySpan<char> Line { get; }
            public ReadOnlySpan<char> Separator { get; }

            // This method allow to deconstruct the type, so you can write any of the following code
            // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
            // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
            // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
            public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
            {
                line = Line;
                separator = Separator;
            }

            // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
            // foreach (ReadOnlySpan<char> entry in str.SplitLines())
            public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
        }
    }
}
