using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    /// <summary>
    /// Finds the distance to the closest different-valued element for every element in the array.
    /// Distance is euclidean. 
    /// </summary>
    [GlobalClass, Tool]
    public partial class NearestNeighbours : Algorithm
    {
        [Export] public BlendMode Mode
        {
            get { return _mode; }
            set { _mode = value; PropertyValueChanged.Invoke(); }
        }
        private BlendMode _mode;

        [Export] public int Iterations
        {
            get { return _iterations; }
            set { _iterations = value; PropertyValueChanged.Invoke(); }
        }
        private int _iterations = 1;

        // [Methods]
        // ****************************************************************************************************
        protected override float[,] Process(float[,] input, float sampleSize)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            float[,] output = input;

            for (int iter = 0; iter < Iterations; iter++)
            {
                float[,] iterationOutput = new float[width, height];

                // Loops through for each element in the array
                Parallel.For(0, width * height, i =>
                {
                    int x = i % width;
                    int y = SQMath.DivFloor(i, width);

                    float[] neighbours = new float[8];
                    int n = 0;
                    for (int v = Math.Clamp(y - 1, 0, height - 1); v <= Math.Clamp(y + 1, 0, height - 1); v++)
                    for (int u = Math.Clamp(x - 1, 0, width - 1); u <= Math.Clamp(x + 1, 0, width - 1); u++)
                    {
                        if (x == u && y == v) continue;
                        neighbours[n] = output[u, v];
                        n++;
                    }

                    switch (Mode)
                    {
                        case BlendMode.Constant: iterationOutput[x, y] = neighbours.GroupBy(nb => nb).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First(); break;
                        case BlendMode.Linear: iterationOutput[x, y] = neighbours.Take(n).Sum() / n; break;
                    }
                });

                output = iterationOutput;
            }

            return output;
        }

        public enum BlendMode
        {
            Constant,
            Linear
        }
    }
}