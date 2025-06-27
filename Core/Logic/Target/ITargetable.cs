using Godot;
using SQGame.Logic.Behaviour;
using System;

namespace SQGame.Logic.Target
{
    public interface ITargetable
    {
        public EntityTransform Transform { get; }
        public bool IsActive { get; }
    }
}