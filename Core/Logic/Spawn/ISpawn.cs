using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Logic.Spawn
{
    /// <summary>
    /// Dictates initial transform of the entity
    /// </summary>
    public interface ISpawn
    {
        public EntityTransform[] Get(int count);
    }
}