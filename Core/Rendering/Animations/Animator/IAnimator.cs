using SQGame.Logic.Target;
using SQGame.Rendering.Components;

namespace SQGame.Rendering.Animations.Animator
{
    public interface IAnimator
    {
        public void UpdateAnimation(AnimationPlayer player, EntityTransform newTransform, EntityTransform oldTransform);
    }
}