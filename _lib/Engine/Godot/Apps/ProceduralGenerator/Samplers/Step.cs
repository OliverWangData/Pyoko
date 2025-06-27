using Godot;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Step : Resampler
    {
        // [Godot]
        // ****************************************************************************************************
        [Export(PropertyHint.Range, "0,1")] public float Threshold
        {
            get { return _threshold; }
            set { _threshold = value; PropertyValueChanged.Invoke(); }
        }
        private float _threshold = 0.5f;

        // [Methods]
        // ****************************************************************************************************
        protected override float Resample(float value)
        {
            return value >= Threshold? 1 : 0;
        }
    }
}