#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.Text;

namespace TrafficCourts.Common.Converters;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// From https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/StringUtils.cs
/// </remarks>
internal class JsonUtils
{
    private enum SeparatedCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }

    public static string ToSnakeCase(string s) => ToSeparatedCase(s, '_');

    private static string ToSeparatedCase(string s, char separator)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        StringBuilder sb = new StringBuilder();
        SeparatedCaseState state = SeparatedCaseState.Start;

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ')
            {
                if (state != SeparatedCaseState.Start)
                {
                    state = SeparatedCaseState.NewWord;
                }
            }
            else if (char.IsUpper(s[i]))
            {
                switch (state)
                {
                    case SeparatedCaseState.Upper:
                        bool hasNext = (i + 1 < s.Length);
                        if (i > 0 && hasNext)
                        {
                            char nextChar = s[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != separator)
                            {
                                sb.Append(separator);
                            }
                        }
                        break;
                    case SeparatedCaseState.Lower:
                    case SeparatedCaseState.NewWord:
                        sb.Append(separator);
                        break;
                }

                char c;
                c = char.ToLowerInvariant(s[i]);
                sb.Append(c);

                state = SeparatedCaseState.Upper;
            }
            else if (s[i] == separator)
            {
                sb.Append(separator);
                state = SeparatedCaseState.Start;
            }
            else
            {
                if (state == SeparatedCaseState.NewWord)
                {
                    sb.Append(separator);
                }

                sb.Append(s[i]);
                state = SeparatedCaseState.Lower;
            }
        }

        return sb.ToString();
    }
}