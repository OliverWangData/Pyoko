using Godot;
using SQLib.Extensions;
using SQLib.Procedural;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Noise : Sampler
    {
        [Export] public NoiseSampler.BaseNoiseType NoiseType
        {
            get { return _noiseType; }
            set { _noiseType = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private NoiseSampler.BaseNoiseType _noiseType;

        [Export] public float Scale
        {
            get { return _scale; }
            set { _scale = value; PropertyValueChanged.Invoke(); }
        }
        private float _scale = 1;

        [Export] public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _offset;

        [Export] public float Seed
        {
            get { return _seed; }
            set { _seed = value; PropertyValueChanged.Invoke(); }
        }
        private float _seed;

        // [Godot]
        // ****************************************************************************************************
        // Hide certain export fields based on conditions
        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            bool remove = false;
            string name = property["name"].As<string>();

            if (_noiseType == NoiseSampler.BaseNoiseType.Random && name.IsIn(new string[] { "Scale", "Offset" })) remove = true;     // Random doesn't use scale or offset
            if (remove) property["usage"] = Variant.From(PropertyUsageFlags.NoEditor);
        }

        // [Methods]
        // ****************************************************************************************************
        public override float Sample(float x, float y)
        {
            switch (_noiseType)
            {
                case NoiseSampler.BaseNoiseType.Random: return NoiseSampler.Random(x, y, Seed);
                case NoiseSampler.BaseNoiseType.Perlin: return NoiseSampler.Perlin(x, y, Scale, Offset.X, Offset.Y, Seed);
                case NoiseSampler.BaseNoiseType.CellularValue: return NoiseSampler.Cellular(x, y, Scale, Offset.X, Offset.Y, Seed).Value;
                case NoiseSampler.BaseNoiseType.CellularDistance: return NoiseSampler.Cellular(x, y, Scale, Offset.X, Offset.Y, Seed).DistCenter;
                default: return 0;
            }
        }
    }
}