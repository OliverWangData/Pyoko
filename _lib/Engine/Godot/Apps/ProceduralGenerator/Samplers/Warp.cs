using Godot;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Warp : Sampler
    {
        [Export] public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _offset;

        [Export(PropertyHint.Range, "0,10")] public int Layers
        {
            get { return _layers; }
            set { _layers = value; PropertyValueChanged.Invoke(); }
        }
        private int _layers = 1;

        [Export] public float Strength
        {
            get { return _strength; }
            set { _strength = value; PropertyValueChanged.Invoke(); }
        }
        private float _strength = 1;

        [Export] public Sampler BaseSampler
        {
            get { return _baseSampler; }
            set
            {
                _baseSampler?.PropertyValueChanged.Remove(PropertyValueChanged);
                _baseSampler = value;
                _baseSampler?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _baseSampler;

        [Export]
        public Sampler ShiftSampler
        {
            get { return _shiftSampler; }
            set
            {
                _shiftSampler?.PropertyValueChanged.Remove(PropertyValueChanged);
                _shiftSampler = value;
                _shiftSampler?.PropertyValueChanged.Add(PropertyValueChanged);
            }
        }
        private Sampler _shiftSampler;

        // [Methods]
        // ****************************************************************************************************
        public override float Sample(float x, float y)
        {
            // Sampling Parameters
            float shiftX = x + Offset.X;
            float shiftY = y + Offset.Y;

            // Domain Warp Layers
            for (int i = 0; i < Layers; i++)
            {
                float warpShiftX = ShiftSampler.Sample(shiftX + 0, shiftY + 0);
                float warpShiftY = ShiftSampler.Sample(shiftX + 5.2f, shiftY + 1.3f);
                shiftX += (warpShiftX * 2 - 1) * Strength;
                shiftY += (warpShiftY * 2 - 1) * Strength;
            }

            return BaseSampler.Sample(shiftX, shiftY);
        }
    }
}