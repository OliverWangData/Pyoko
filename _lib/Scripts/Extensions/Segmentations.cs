using System.Collections.Generic;

namespace SQLib.Extensions
{
    public static class Segmentations
    {
        // [Arrays]
        // ****************************************************************************************************
        /// <summary>
        /// Slicing of 2D array. Returns a copy of the slice. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="xStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yStart"></param>
        /// <param name="yEnd"></param>
        /// <returns></returns>
        public static T[,] Slice<T>(this T[,] array, int xStart, int xEnd, int yStart, int yEnd, bool inclusive = false)
        {
            int xSize = xEnd - xStart + ((inclusive) ? 1 : 0);
            int ySize = yEnd - yStart + ((inclusive) ? 1 : 0);
            T[,] slice = new T[xSize, ySize];

            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    slice[x, y] = array[x + xStart, y + yStart];
                }
            }

            return slice;
        }

        public static T[] GetSubArray<T>(this T[] array, int start, int length)
        {
            T[] subArray = new T[length];
            System.Array.Copy(array, start, subArray, 0, length);
            return subArray;
        }
    }
}
