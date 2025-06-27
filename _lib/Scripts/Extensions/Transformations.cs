using System.Collections.Generic;
using System;
using Godot;

namespace SQLib.Extensions
{
    public static class Transformations
    {
        // [Values]
        // ****************************************************************************************************
        /// <summary>
        /// Scales a value from the range [a, b] to the range [x, y]
        /// </summary>
        /// <param name="value">        Value to scale. </param>
        /// <param name="a">            Lower value of initial range. </param>
        /// <param name="b">            Upper value of initial range. </param>
        /// <param name="x">            Lower value of new range. </param>
        /// <param name="y">            Upper value of new range. </param>
        /// <returns></returns>
        public static float ScaleRange(this float value, float a, float b, float x, float y)
        {
            if ((b - a) == 0)
            {
                return value;
            }
            else
            {
                return value - ((b + a) / 2) * ((y - x) / (b - a)) + ((y + x) / 2);
            }
        }

        // [Vectors]
        // ****************************************************************************************************
        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            float angle = Mathf.DegToRad(-degrees);
            float x = (Mathf.Cos(angle) * vector.X) - (Mathf.Sin(angle) * vector.Y);
            float y = (Mathf.Sin(angle) * vector.X) + (Mathf.Cos(angle) * vector.Y);
            return new Vector2(x, y);
        }
    }
}
