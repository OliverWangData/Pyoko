using Godot;
using System;

namespace SQGame.Rendering.Animations
{
    [GlobalClass]
    public partial class AnimationInfo: Resource 
    {
        [Export] public AnimationState State;
        [Export] public float Fps;
        [Export] public Vector2 Size;
        [Export] public Vector2[] Positions;
    }
}