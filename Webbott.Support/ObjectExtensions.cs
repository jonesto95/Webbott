using System;
using System.Collections.Generic;
using System.Text;

namespace Webbott.Support
{
    public static class ObjectExtensions
    {
        public static string MergeWithDelimiter<T>(this T[] array, string delimiter)
        {
            string result = "";
            for(int i = 0; i < array.Length; i++)
                result += $"{array[i]}{delimiter}";

            result = result[..^delimiter.Length];
            return result;
        }


        public static bool Contains(this string input, char c)
            => input.Contains($"{c}");


        public static bool Contains(this string input, params char[] chars)
        {
            foreach(char c in chars)
                if (input.Contains(c))
                    return true;

            return false;
        }


        public static bool Contains(this string input, params string[] strings)
        {
            foreach(string s in strings)
                if (input.Contains(s))
                    return true;

            return false;
        }


        public static string SafeSubstring(this string input, int startIndex)
        {
            if (input.Length > startIndex)
                return input.Substring(startIndex);

            return "";
        }


        public static string SafeSubstring(this string input, int startIndex, int length)
        {
            string result = input.SafeSubstring(startIndex);
            if (result.Length > length)
                return result.Substring(0, length);

            return result;
        }


        public static string TrimEnd(this string input, int trimLength)
            => input.SafeSubstring(0, input.Length - trimLength);


        public static string TrimStart(this string input, int trimAmount)
            => input.SafeSubstring(trimAmount);


        public static bool In<T>(this T input, params T[] values)
        {
            foreach(T value in values)
                if(input.Equals(value))
                    return true;

            return false;
        }


        public static U GetKeyOrDefault<T, U>(this Dictionary<T, U> input, T keyValue, U defaultValue)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            U result = defaultValue;
            if (input.ContainsKey(keyValue))
                result = input[keyValue];

            return result;
        }


        public static int Sign(this int input)
        {
            if (input < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
