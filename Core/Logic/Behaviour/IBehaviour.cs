using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Logic.Behaviour
{
    /// <summary>
    /// Behaviours apply changes to direction, scale, and rotation over time.
    /// The behaviour owners will need to calculate the final position using these.
    /// </summary>
    public interface IBehaviour
    {
        public EntityTransform Process(EntityTransform initialTransform, double delta);
    }
}