using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Logic.Spawn
{
    public class Point : ISpawn
    {
        private ITargetable origin;
        private ITargetable target;
        public Point(ITargetable origin, ITargetable target)
        {
            this.origin = origin;
            this.target = target;
        }

        public EntityTransform[] Get(int count)
        {
            EntityTransform[] transforms = new EntityTransform[count];

            for (int i = 0; i < count; i++)
            {
                EntityTransform transform = origin.Transform;
                if (target is not null & target.IsActive) transform.Direction = (target.Transform.Position - origin.Transform.Position).Normalized();
                transforms[i] = transform;
            }

            return transforms;
        }
    }
}
