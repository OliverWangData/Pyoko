using Godot;
using System;

namespace SQLib.Input
{
    public class DiscreteDoubleAxis
    {

        public bool Up
        {
            get { return _upDown.A; }
            set { _upDown.A = value; }
        }
        public bool Down
        {
            get { return _upDown.B; }
            set { _upDown.B = value; }
        }
        public bool Left
        {
            get { return _leftRight.A; }
            set { _leftRight.A = value; }
        }
        public bool Right
        {
            get { return _leftRight.B; }
            set { _leftRight.B = value; }
        }

        private DiscreteAxis _upDown = new();
        private DiscreteAxis _leftRight = new();

        public Vector2 Get()
        {
            return new Vector2(_leftRight.Get(), _upDown.Get()).Normalized();
        }
    }
}