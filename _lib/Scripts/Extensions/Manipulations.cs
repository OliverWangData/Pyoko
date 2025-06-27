using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLib.Extensions
{
    public static class Manipulations
    {
        // [Strings]
        // ****************************************************************************************************
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            char[] array = input.ToCharArray();
            array[0] = char.ToUpper(array[0]);
            return new string(array);
        }

        // [Arrays]
        // ****************************************************************************************************
        public static void AddTo<T>(this T item, ref T[] arr)
        {
            arr = arr.Concat(new T[] { item }).ToArray();
        }
        public static void ArrayAdd<T>(ref T[] arr, T item)
        {
            arr = arr.Concat(new T[] { item }).ToArray();
        }
    }
}
