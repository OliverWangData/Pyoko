using Godot;
using System;

namespace SQGame.Rendering.Animations.Animator
{
    public class AnimatorFactory
    {
        // [Initialization]
        // ****************************************************************************************************
        public AnimatorFactory()
        {

        }

        // [Methods]
        // ****************************************************************************************************

        public IAnimator Get(Animator type)
        {
            switch (type)
            {
                case Animator.None:
                    return null;

                case Animator.LeftRight:
                    return new LeftRight();

                case Animator.Character:
                    return new Character();

                default:
                    throw new NotImplementedException($"{type} is not implemented.");
            }
        }

        public enum Animator
        {
            None,
            LeftRight,
            Character
        }
    }

}