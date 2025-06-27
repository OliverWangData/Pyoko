using Godot;
using System;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    /// <summary>
    /// </summary>
    [GlobalClass, Tool]
    public partial class CircleFade : Algorithm
    {
        [Export] public float CircleRadius
        {
            get { return _circleRadius; }
            set { _circleRadius = value; PropertyValueChanged.Invoke(); }
        }
        private float _circleRadius = 10;

        [Export] public float FadeSize
        {
            get { return _fadeSize; }
            set { _fadeSize = value; PropertyValueChanged.Invoke(); }
        }
        private float _fadeSize = 5;

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

                float distCenter = MathF.Sqrt((x - width / 2) * (x - width / 2) + (y - height / 2) * (y - height / 2));
                output[x, y] = input[x, y] * Mathf.Clamp(SQMath.Lerp(1, 0, (distCenter - (CircleRadius - FadeSize)) / FadeSize), 0, 1);
            });

            return output;
        }
    }
}