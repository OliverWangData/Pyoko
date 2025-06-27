using Godot;

namespace SQLib.GDEngine.ProceduralGenerator
{
    public abstract partial class Algorithm : Sampler
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public Sampler Sampler
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
        public sealed override float Sample(float x, float y) => Sample(1, 1, x, y, 1)[0, 0];

        public sealed override float[,] Sample(int width, int height, float startX, float startY, float sampleSize)
        {
            if (Sampler == null) return new float[width, height];

            float[,] sample = Sampler.Sample(width, height, startX, startY, sampleSize);
            return Process(sample, sampleSize);
        }

        protected abstract float[,] Process(float[,] input, float sampleSize);
    }
}