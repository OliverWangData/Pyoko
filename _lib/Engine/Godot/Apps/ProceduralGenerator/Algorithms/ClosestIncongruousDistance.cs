using Godot;
using System;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    /// <summary>
    /// Finds the distance to the closest different-valued element for every element in the array.
    /// Distance is euclidean. 
    /// </summary>
    [GlobalClass, Tool]
    public partial class ClosestIncongruousDistance : Algorithm
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public int SearchRadius
        {
            get { return _searchRadius; }
            set { _searchRadius = value; PropertyValueChanged.Invoke(); }
        }
        private int _searchRadius;

        // [Methods]
        // ****************************************************************************************************
        protected override float[,] Process(float[,] input, float sampleSize)
        {
            // Trivial case where the entire input array is just one sample value. 
            if (sampleSize == 0) return input;

            // SearchRadius is scaled based on the sample size, so that when the sample size is changed, the search radius is not fixed to "x" number of pixels.
            // If it were fixed, changing the scale of the noise via sampleSize (E.g. How any caller that uses Sampler.Sample(int width, int height, float startX, float startY, float sampleSize) would have to do it)
            // would change the scale of the noise but not of the algorithm (E.g. zooming out the noise would still have very thick algorithm borders as if it was zoomed in).  
            int radius = SQMath.Round(SearchRadius / sampleSize);

            if (radius == 0) return input;
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            float[,] output = new float[width, height];

            // Loops through for each element in the array
            Parallel.For(0, width * height, i =>
            {
                int x = i % width;
                int y = SQMath.DivFloor(i, width);

                // Searchest for closest different-valued element
                int shortestDistSquared = 2 * radius.Pow(2);

                for (int v = Math.Clamp(y - radius, 0, height - 1); v <= Math.Clamp(y + radius, 0, height - 1); v++)
                for (int u = Math.Clamp(x - radius, 0, width - 1); u<= Math.Clamp(x + radius, 0, width - 1); u++)
                {
                    if (x == u && y == v) continue;
                    if (input[x, y] == input[u, v]) continue;
                    int distSquared = (u - x).Pow(2) + (v - y).Pow(2);
                    shortestDistSquared = Math.Min(shortestDistSquared, distSquared);
                }

                output[x, y] = Math.Clamp(MathF.Sqrt(shortestDistSquared) / radius, 0, 1f);
            });

            return output;
        }
    }
}