using Godot;
using System;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    /// <summary>
    /// </summary>
    [GlobalClass, Tool]
    public partial class BorderFade : Algorithm
    {
        [Export] public float BorderSize
        {
            get { return _borderSize; }
            set { _borderSize = value; PropertyValueChanged.Invoke(); }
        }
        private float _borderSize = 0;

        // [Methods]
        // ****************************************************************************************************
        protected override float[,] Process(float[,] input, float sampleSize)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            float[,] output = input;

            // Loops through for each element in the array
            Parallel.For(0, width * height, i =>
            {
                int x = i % width;
                int y = SQMath.DivFloor(i, width);

                if (_borderSize == 0)
                {
                    output[x, y] = input[x, y];
                    return;
                }

                float fracX = MathF.Min(SQMath.Lerp(0, 1, x / BorderSize), SQMath.Lerp(0, 1, (width - x) / BorderSize));
                float fracY = MathF.Min(SQMath.Lerp(0, 1, y / BorderSize), SQMath.Lerp(0, 1, (height - y) / BorderSize));
                output[x, y] = input[x, y] * MathF.Min(fracX, MathF.Min(fracY, 1));
            });

            return output;
        }
    }
}