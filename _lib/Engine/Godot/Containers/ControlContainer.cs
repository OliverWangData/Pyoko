using Godot;
using System;

namespace SQLib.GDEngine.Containers
{
    [GlobalClass, Tool]
    public abstract partial class ControlContainer : Control
    {
        [Export] public bool Refresh
        {
            get { return false; }
            set { if (value) Update(); }
        }

        [ExportGroup("Settings")]
        [Export] public bool PixelSnap
        {
            get { return _pixelSnap; }
            set
            {
                _pixelSnap = value;
                Update();
            }
        }
        private bool _pixelSnap = true;

        [Export] public bool EnableDraw
        {
            get { return _enableDraw; }
            set
            {
                _enableDraw = value;
                QueueRedraw();
            }
        }
        private bool _enableDraw = true;

        [Export] public Color DrawColor
        {
            get { return _drawColor; }
            set
            {
                _drawColor = value;
                QueueRedraw();
            }
        }
        private Color _drawColor = new Color(1, 0, 0, 1);

        public abstract void Update();
    }
}
