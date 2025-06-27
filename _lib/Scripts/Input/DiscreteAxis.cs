using Godot;
using System;

namespace SQLib.Input
{
    public class DiscreteAxis
    {
        public bool A;
        public bool B;

        public float Get()
        {
            if (A && !B) return -1;
            if (!A && B) return 1;
            return 0;
        }
    }
}