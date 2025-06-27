using System;
using System.Collections.Generic;

namespace SQLib
{
    public static class SQMath
    {
        // [Native Extension]
        // ****************************************************************************************************
        // Extends already implemented C# Math functions that are missing certain inputs and outputs types
        public static int Round(this float x) => (int)System.Math.Round(x);
        public static int Pow(this int value, int exp) => (int)System.Math.Pow(value, exp);

        // [Spacing]
        // ****************************************************************************************************
        // Floors a value by to a multiple of the spacing
        public static float FloatFloor(this float value, float spacing)
        {
            return MathF.Floor(value / spacing) * spacing;
        }

        // Floors integer division
        public static int DivFloor(this int a, int b)
        {
            return a / b;
        }
        
        // Ceilings integer division
        public static int DivCeil(this int a, int b)
        {
            return ((a - 1) / b) + 1;
        }

        // Floors a float into an int
        public static int IntFloor(this float x)
        {
            return (int)MathF.Floor(x);
        }

        // ****************************************************************************************************
        // GLSL Mix implementation
        public static float Lerp(this float x, float y, float a)
        {
            return (x * (1 - a)) + (y * a);
        }

        // GLSL Fract implementation
        public static float Fract(this float x)
        {
            return x - MathF.Floor(x);
        }

        // GLSL Mod implementation
        public static float Mod(this float x, float y)
        {
            return x - y * MathF.Floor(x / y);
        }
    }
}
