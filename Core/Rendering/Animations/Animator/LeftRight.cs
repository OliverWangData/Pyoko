using Godot;
using SQGame.Logic.Target;

namespace SQGame.Rendering.Animations.Animator
{
    public class LeftRight : IAnimator
    {
        public void UpdateAnimation(Components.AnimationPlayer player, EntityTransform newTransform, EntityTransform oldTransform)
        {
            // Flips the animation left or right depending on the new direction
            if (newTransform.Direction.X < 0 & oldTransform.Direction.X >= 0)
            {
                player.FlipCanvas = true;
                newTransform.Rotation = Mathf.Atan2(newTransform.Direction.Y, newTransform.Direction.X) + Mathf.Pi;
            }

            if (newTransform.Direction.X > 0 & oldTransform.Direction.X <= 0)
            {
                player.FlipCanvas = false;
                newTransform.Rotation = Mathf.Atan2(newTransform.Direction.Y, newTransform.Direction.X);
            }
        }
    }
}