using SQGame.Logic.Target;
using Godot;

namespace SQGame.Logic.Behaviour
{
    public class PlayerControllerInput : IBehaviour
    {
        public Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value.Normalized(); }
        }
        private Vector2 _direction;

        public EntityTransform Process(EntityTransform initialTransform, double delta)
        {
            EntityTransform transform = initialTransform;
            transform.Direction = Direction;
            return transform;
        }
    }
}