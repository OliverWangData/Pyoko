using Godot;
using SQGame.Logic.Target;
using SQLib.Extensions;
using System;

namespace SQGame.Logic.Spawn
{
    public class Cone : ISpawn
    {
        private ITargetable origin;
        private ITargetable target;
        private float spread;

        public Cone(ITargetable origin, ITargetable target, float spreadDeg)
        {
            this.origin = origin;
            this.target = target;
            spread = spreadDeg *= MathF.PI / 180;
        }

        public EntityTransform[] Get(int count)
        {
            EntityTransform[] transforms = new EntityTransform[count];

            for (int i = 0; i < count; i++)
            {
                EntityTransform transform = origin.Transform;
                Vector2 targetPosition = (target is not null & target.IsActive) ? target.Transform.Position : origin.Transform.Direction;

                // Spawning only 1 instance. Should aim directly at target
                if (count == 1)
                {
                    transform.Direction = (targetPosition - origin.Transform.Position).Normalized();
                }
                // Spawning multiple. Should be evenly divided between min and max spread angles
                else
                {
                    float ratio = (float)i / (count - 1);
                    float rad = spread * ratio - (spread / 2) + (targetPosition - origin.Transform.Position).ToRad();
                    transform.Direction = rad.ToVector2();
                }

                transforms[i] = transform;
            }

            return transforms;
        }
    }
}
