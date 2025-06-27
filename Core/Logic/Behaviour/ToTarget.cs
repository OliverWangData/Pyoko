using Godot;
using SQGame.Logic.Target;

namespace SQGame.Logic.Behaviour
{
    public class ToTarget : IBehaviour
    {
        private ITargetable target;
        private Vector2 lastDirection;

        public ToTarget(ITargetable target)
        {
            this.target = target;
        }

        public EntityTransform Process(EntityTransform initialTransform, double delta)
        {

            EntityTransform transform = initialTransform;

            if (target is not null & target.IsActive)
            {
                transform.Direction = (target.Transform.Position - initialTransform.Position).Normalized();
                lastDirection = transform.Direction;
            }
            else
            {
                transform.Direction = lastDirection;
            }

            return transform;
        }
    }
}