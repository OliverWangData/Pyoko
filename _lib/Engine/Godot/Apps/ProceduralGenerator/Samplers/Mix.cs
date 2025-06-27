using Godot;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Mix : Sampler
    {
        [Export] public MixSource Source
        {
            get { return _source; }
            set { _source = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private MixSource _source;

        [Export(PropertyHint.Range, "0,1")] public float Factor
        {
            get { return _factor; }
            set { _factor = value; PropertyValueChanged.Invoke(); }
        }
        private float _factor;

        [Export] public Sampler SamplerMix
        {
            get { return _samplerMix; }
            set
            {
                _samplerMix?.PropertyValueChanged.Remove(PropertyValueChanged);
                _samplerMix = value;
                _samplerMix?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _samplerMix;

        [Export] public Sampler SamplerA
        {
            get { return _samplerA; }
            set
            {
                _samplerA?.PropertyValueChanged.Remove(PropertyValueChanged);
                _samplerA = value;
                _samplerA?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _samplerA;

        [Export]
        public Sampler SamplerB
        {
            get { return _samplerB; }
            set
            {
                _samplerB?.PropertyValueChanged.Remove(PropertyValueChanged);
                _samplerB = value;
                _samplerB?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _samplerB;

        // [Godot]
        // ****************************************************************************************************
        // Hide certain export fields based on conditions
        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            bool remove = false;
            string name = property["name"].As<string>();

            if (Source == MixSource.Factor && name == "SamplerMix") remove = true;
            else if (Source == MixSource.Sampler && name == "Factor") remove = true;

            if (remove) property["usage"] = Variant.From(PropertyUsageFlags.NoEditor);
        }

        // [Methods]
        // ****************************************************************************************************
        public override float Sample(float x, float y)
        {
            switch (Source)
            {
                case MixSource.Factor: return SQMath.Lerp(SamplerA.Sample(x, y), SamplerB.Sample(x, y), System.Math.Clamp(Factor, 0, 1));
                case MixSource.Sampler: return SQMath.Lerp(SamplerA.Sample(x, y), SamplerB.Sample(x, y), System.Math.Clamp(SamplerMix.Sample(x, y), 0, 1));
                default: return 0;
            }
        }

        // [Enums]
        // ****************************************************************************************************
        public enum MixSource
        {
            Factor,
            Sampler
        }
    }
}