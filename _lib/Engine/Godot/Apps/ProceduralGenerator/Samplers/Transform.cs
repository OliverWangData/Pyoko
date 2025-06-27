using Godot;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Transform : Sampler
    {
        // [Godot]
        // ****************************************************************************************************
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
            return Sampler.Sample(x / Scale + Offset.X, y / Scale + Offset.Y);
        }
    }
}