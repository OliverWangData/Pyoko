using Godot;

namespace SQGame.Logic.Target
{
    public struct EntityTransform
    {
        // [Fields]
        // ****************************************************************************************************
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;
        public Vector2 Direction;

        // [Constructors]
        // ****************************************************************************************************
        public EntityTransform()
        {
            Position = new Vector2(0, 0);
            Rotation = 0;
            Scale = new Vector2(1, 1);
            Direction = new Vector2(0, 0);
        }

        public EntityTransform(Vector2 position, float rotation, Vector2 scale, Vector2 direction)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Direction = direction;
        }

        public Transform2D GetTransform2D()
        {
            return new(Rotation, Scale, 0, Position);
        }
    }
}