using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class MultiStep : Resampler
    {
        // [Godot]
        // ****************************************************************************************************
        [Export(PropertyHint.Range, "0,1")] public float[] Thresholds
        {
            get { return _thresholds; }
            set 
            {
                if (!value.Contains(1)) value = value.Concat(new float[] { 1 }).ToArray();
                _thresholds = value.OrderBy(x => x).ToArray(); 
                PropertyValueChanged.Invoke(); 
            }
        }
        private float[] _thresholds;

        // [Methods]
        // ****************************************************************************************************
        protected override float Resample(float value)
        {
            for (int i = 0; i < Thresholds.Length; i++)
            {
                if (Thresholds[i] >= value)
                {
                    return (float)(i + 1) / Thresholds.Length;
                }
            }

            return 0;
        }
    }
}