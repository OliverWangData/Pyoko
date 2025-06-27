using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLib.Extensions
{
    public static class Conversions
    {
        // [Vectors]
        // ****************************************************************************************************
        public static Godot.Vector2 ToVector2(this float rad)
        {
            return new Godot.Vector2(MathF.Cos(rad), MathF.Sin(rad));
        }

        public static float ToRad(this Godot.Vector2 direction)
        {
            return MathF.Atan2(direction.Y, direction.X);
        }

        // [Strings]
        // ****************************************************************************************************
        public static T ToType<T>(this string value, bool emptyToDefault = false)
        {
            if (value == "" & emptyToDefault)
            {
                return default;
            }

            if (typeof(T).IsEnum)
            {
                // Enums are default backed by int32 but can be overriden to use other numeric backings. 
                // This only implements int32 backings. Adding other backings currently requires reflections. 
                string[] flags = value.Split('|');
                int output = default;

                for (int i = 0; i < flags.Length; i++)
                {
                    int flag = (int) Enum.Parse(typeof(T), flags[i]);
                    output = output | flag;
                }

                return (T)(object)output;
            }
            else
            {
                try { return (T)Convert.ChangeType(value, typeof(T)); }
                catch (FormatException) 
                {
                    throw new ArgumentException($"Could not convert '{value}' to type '{typeof(T)}'.");
                }
            }
        }

        public static bool IsType<T>(this string value)
        {
            return value.IsType(typeof(T));
        }

        public static bool IsType(this string value, Type type)
        {

            if (value == "" & type != typeof(string))
            {
                return false;
            }

            if (type.IsEnum)
            {
                try { var _ = Enum.Parse(type, value); }
                catch (FormatException) { return false; }
                return true;
            }
            else
            {
                try { var _ = Convert.ChangeType(value, type); }
                catch (FormatException) { return false; }
                return true;
            }
        }

        // [Arrays]
        // ****************************************************************************************************
        public static T[,] To2DArray<T>(this IEnumerable<T> enumerable, int width = 1, int height = 1)
        {
            T[,] outputArray = new T[width, height];

            // Convert the 1D array back to 2D array
            int xIndex = 0;
            int yIndex = 0;
            foreach (T t in enumerable)
            {
                outputArray[xIndex, yIndex] = t;
                xIndex++;

                if (xIndex == width)
                {
                    xIndex = 0;
                    yIndex++;
                }
            }

            return outputArray;
        }

        public static T[] ToRepeatArray<T>(this T value, int count = 1)
        {
            T[] outputArray = new T[count];
            for (int i = 0; i < count; i++) outputArray[i] = value;
            return outputArray;
        }

        public static T[] ToRepeatArray<T>(this Func<T> func, int count = 1)
        {
            T[] outputArray = new T[count];
            for (int i = 0; i < count; i++) outputArray[i] = func();
            return outputArray;
        }
    }
}
