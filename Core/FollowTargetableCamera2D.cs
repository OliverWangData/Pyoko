using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Core
{
    public partial class FollowTargetableCamera2D : Camera2D, ITargetable
    {
        public new EntityTransform Transform
        {
            get
            {
                return new(GlobalPosition, RotationDegrees, Scale, Target.Transform.Direction);
            }
        }

        public ITargetable Target;

        public bool IsActive
        {
            get
            {
                return IsInsideTree();
            }
        }

        public override void _Process(double delta)
        {
            if (Target is not null & Target.IsActive)
            {
                GlobalPosition = Target.Transform.Position;
            }
        }
    }
}
