using Godot;
using System;
using System.Collections.Generic;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class Ridge : Resampler
    {
        // [Methods]
        // ****************************************************************************************************
        protected override float Resample(float value)
        {
            return MathF.Abs(value * 2 - 1) + 1 / 2;
        }

    }
}