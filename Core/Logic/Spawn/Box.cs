using Godot;
using SQGame.Logic.Target;

namespace SQGame.Logic.Spawn
{
    public class Box : ISpawn
    {
        // [Fields]
        // ****************************************************************************************************
        private ITargetable origin;
        private ITargetable target;
        private Vector2 size;
        private RandomNumberGenerator rng;

        // [Initialization]
        // ****************************************************************************************************
        public Box(ITargetable origin, ITargetable target, Vector2 boxSize)
        {
            this.origin = origin;
            this.target = target;
            size = boxSize;
            rng = new();
        }

        // [Properties]
        // ****************************************************************************************************

        public float Perimeter
        {
            get { return size.X * 2 + size.Y * 2; }
        }

        // [Interface]
        // ****************************************************************************************************
        public EntityTransform[] Get(int count)
        {
            EntityTransform[] transforms = new EntityTransform[count];
            float spacing = Perimeter / count;

            float startDist = rng.RandfRange(0, Perimeter);

            for (int i = 0; i < count; i++)
            {
                EntityTransform transform =  new();
                transform.Position = GetPositionOnBox(startDist + (spacing * i)) - (size / 2) + origin.Transform.Position;
                if (target is not null & target.IsActive) transform.Direction = (target.Transform.Position - transform.Position).Normalized();
                transforms[i] = transform;
            }

            return transforms;
        }

        // [Methods]
        // ****************************************************************************************************
        /// <summary>
        /// Gets the relative position along the box, going from (0, 0) to (1, 0) to (1, 1) to (0, 1)
        /// </summary>
        /// <param name="dist"></param>
        private Vector2 GetPositionOnBox(float dist)
        {
            dist %= Perimeter;

            if (dist < size.X) return new Vector2(dist, 0);
            dist -= size.X;

            if (dist < size.Y) return new Vector2(size.X, dist);
            dist -= size.Y;

            if (dist < size.X) return new Vector2(size.X - dist, size.Y);
            dist -= size.X;

            return new Vector2(0, size.Y - dist);
        }
    }
}