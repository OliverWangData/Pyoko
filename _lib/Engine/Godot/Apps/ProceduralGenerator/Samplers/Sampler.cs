using Godot;
using SQLib.Events;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    public abstract partial class Sampler : Resource
    {
        // [Fields]
        // ****************************************************************************************************
        public Callback PropertyValueChanged { get; set; } = new();

        // [Methods]
        // ****************************************************************************************************
        public abstract float Sample(float x, float y);

        public virtual float[,] Sample(int width, int height, float startX, float startY, float sampleSize)
        {
            float[,] data = new float[width, height];

            Parallel.For(0, width * height, i =>
            {
                int x = i % width;
                int y = SQMath.DivFloor(i, width);
                float sampleX = startX + (sampleSize * x);
                float sampleY = startY + (sampleSize * y);
                data[x, y] = Sample(sampleX, sampleY);
            });

            return data;
        }
    }
}
