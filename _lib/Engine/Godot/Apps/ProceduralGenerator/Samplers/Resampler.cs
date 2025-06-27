using Godot;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    public abstract partial class Resampler : Sampler
    {
        // [Fields]
        // ****************************************************************************************************
        [Export]
        public Sampler Sampler
        {
            get { return _sampler; }
            set
            {
                _sampler?.PropertyValueChanged.Remove(PropertyValueChanged);
                _sampler = value;
                _sampler?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _sampler;

        // [Methods]
        // ****************************************************************************************************

        public sealed override float Sample(float x, float y) => Resample(_sampler.Sample(x, y));

        public sealed override float[,] Sample(int width, int height, float startX, float startY, float sampleSize)
        {
            if (_sampler is Algorithm || _sampler is Resampler)
            {
                float[,] data = _sampler.Sample(width, height, startX, startY, sampleSize);

                Parallel.For(0, width * height, i =>
                {
                    int x = i % width;
                    int y = SQMath.DivFloor(i, width);
                    data[x, y] = Resample(data[x, y]);
                });

                return data;
            }

            else
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

        protected abstract float Resample(float value);
    }
}