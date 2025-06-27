using Godot;
using SQGame.Logic.Target;
using SQLib.Extensions;
using System;

namespace SQGame.Logic.Spawn
{
    public class Radial : ISpawn
    {
        private ITargetable origin;
        private ITargetable target;

        public Radial(ITargetable origin, ITargetable target)
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
                Vector2 targetPosition = (target is not null & target.IsActive) ? target.Transform.Position : origin.Transform.Direction;

                float ratio = (float)i / count;
                float rad = ratio * 2 * MathF.PI + (targetPosition - origin.Transform.Position).ToRad();
                transform.Direction = rad.ToVector2();

                transforms[i] = transform;
            }

            return transforms;
        }
    }
}
