using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Rendering.Components
{
    public interface IRenderComponent : IDisposable
    {
        public EntityTransform Transform { get; }
        public void SetRenderTransform(EntityTransform transform);
        public void SetDrawIndex(int index);
    }
}