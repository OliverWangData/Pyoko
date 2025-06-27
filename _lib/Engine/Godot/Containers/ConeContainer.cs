using Godot;
using System;
using System.Linq;

namespace SQLib.GDEngine.Containers
{
    [GlobalClass, Tool]
    public partial class ConeContainer : ControlContainer
    {
#if TOOLS
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

        [Export] public float Spread
        {
            get { return _spread; }
            set
            {
                _spread = value;
                Update();
            }
        }
        private float _spread = 90;

        // [Godot]
        // ****************************************************************************************************
        public override void _Draw()
        {
            if (Engine.IsEditorHint() && EnableDraw)
            {
                float startAngle = ((-Spread / 2) + AngleOffset) * (MathF.PI / 180);
                float endAngle = ((Spread / 2) + AngleOffset) * (MathF.PI / 180);

                DrawArc(new Vector2(0, 0), Radius, startAngle, endAngle, 32, DrawColor);
                DrawLine(new Vector2(0, 0), Radius * new Vector2(MathF.Cos(startAngle), MathF.Sin(startAngle)), DrawColor);
                DrawLine(new Vector2(0, 0), Radius * new Vector2(MathF.Cos(endAngle), MathF.Sin(endAngle)), DrawColor);
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

            // In the case where there is only one child, the angle for the child should just be equal to the offset angle.
            float anglePerChild = (controls.Length == 1) ? 0 : (Spread * MathF.PI / 180) / (controls.Length - 1);

            for (int i = 0; i < controls.Length; i++)
            {
                float angle = (controls.Length == 1) ? 0 : (anglePerChild * i) - (Spread * MathF.PI / 180 / 2) + (AngleOffset * MathF.PI / 180);
                Vector2 pos = Radius * new Vector2(Mathf.Cos(angle), MathF.Sin(angle));
                if (PixelSnap) pos = new Vector2(MathF.Round(pos.X), MathF.Round(pos.Y));
                controls[i].Position = pos - controls[i].Size / 2;
            }

            QueueRedraw();
        }
#endif
    }
}
