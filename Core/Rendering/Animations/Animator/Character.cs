using Godot;
using SQGame.Logic.Target;

namespace SQGame.Rendering.Animations.Animator
{
    public class Character : IAnimator
    {
        public void UpdateAnimation(Components.AnimationPlayer player, EntityTransform newTransform, EntityTransform oldTransform)
        {
            // Stopping
            if (newTransform.Direction == Vector2.Zero & player.CurrentAnimation != 0)
            {
                player.Play(AnimationState.Default);
            }

            else if (newTransform.Direction != Vector2.Zero & player.CurrentAnimation != AnimationState.Move)
            {
                player.Play(AnimationState.Move);
            }

            // Moving left
            if (newTransform.Direction.X < 0)
            {
                player.FlipCanvas = true;
            }

            // Moving right
            else if (newTransform.Direction.X > 0)
            {
                player.FlipCanvas = false;
            }
        }
    }
}