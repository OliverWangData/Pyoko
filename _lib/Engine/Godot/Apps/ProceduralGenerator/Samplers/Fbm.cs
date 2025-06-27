using Godot;
using SQLib.Events;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Fbm : Sampler
    {
        [Export] public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _offset;

        [Export(PropertyHint.Range, "0,10")] public int Octaves
        {
            get { return _octaves; }
            set { _octaves = value; PropertyValueChanged.Invoke(); }
        }
        private int _octaves = 4;

        [Export] public float Persistance
        {
            get { return _persistance; }
            set { _persistance = value; PropertyValueChanged.Invoke(); }
        }
        private float _persistance = 0.5f;

        [Export] public float Lacunarity
        {
            get { return _lacunarity; }
            set { _lacunarity = value; PropertyValueChanged.Invoke(); }
        }
        private float _lacunarity = 2f;

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
        public override float Sample(float x, float y)
        {
            // Sampling Parameters
            Vector2 coords = new Vector2(x, y) + new Vector2(Offset.X, Offset.Y);
            float value = 0;
            float minVal = 0;
            float maxVal = 0;
            float amplitude = 1;
            float frequency = 1;

            // Brownian octaves
            for (int i = 0; i < Octaves; i++)
            {
                float sample = Sampler.Sample(coords.X * frequency, coords.Y * frequency);

                // The *2-1 Changes range from [0, 1] to [-1, 1].
                value += (sample * 2f - 1f) * amplitude; // Summation of each sample value as they go up octaves

                minVal -= amplitude;
                maxVal += amplitude;

                amplitude *= Persistance; // Amplitude decreases as the octaves go up as persistance [0, 1]
                frequency *= Lacunarity; // Frequency increases as octaves go up as frequency [1, inf)
            }

            if (maxVal - minVal == 0) return 0;
            return (value - ((maxVal + minVal) / 2f)) / (maxVal - minVal) + 0.5f;
        }

    }
}