using Godot;
using System;
using System.Linq;

namespace SQLib.GDEngine.Containers
{
    [GlobalClass, Tool]
    public partial class CircleContainer : ControlContainer
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                Update();
            }
        }
        private float _radius = 100;

        [Export] public float AngleOffset
        {
            get { return _angleOffset; }
            set
            {
                _angleOffset = value;
                Update();
            }
        }
        private float _angleOffset = 0;

        // [Godot]
        // ****************************************************************************************************
        public override void _Draw()
        {
            if (Engine.IsEditorHint() && EnableDraw)
            {
                DrawArc(new Vector2(0, 0), Radius, 0, 2 * MathF.PI, 32, DrawColor);
            }
        }

        // [Methods]
        // ****************************************************************************************************
        public override void Update()
        {
            if (!Engine.IsEditorHint())
            {
                return;
            }

            Control[] controls = GetChildren()
                .Where(node => typeof(Control).IsAssignableFrom(node.GetType()))
                .Select(node => (Control)node)
                .ToArray();

            float angle = (360 * MathF.PI / 180) / controls.Length;

            for (int i = 0; i < controls.Length; i++)
            {
                float currentAngle = (angle * i) - (angle / 2) + (AngleOffset * MathF.PI / 180);
                Vector2 pos = Radius * new Vector2(Mathf.Cos(currentAngle), MathF.Sin(currentAngle));
                if (PixelSnap) pos = new Vector2(MathF.Round(pos.X), MathF.Round(pos.Y));
                controls[i].Position = pos - controls[i].Size / 2;
            }

            QueueRedraw();
        }
    }
}
