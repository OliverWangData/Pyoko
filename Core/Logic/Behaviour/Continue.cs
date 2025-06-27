using Godot;
using SQGame.Logic.Target;

namespace SQGame.Logic.Behaviour
{
    public class Continue : IBehaviour
    {
        public EntityTransform Process(EntityTransform initialTransform, double delta)
        {
            return initialTransform;
        }
    }
}