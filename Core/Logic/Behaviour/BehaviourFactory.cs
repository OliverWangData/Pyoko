using Godot;
using SQGame.Logic.Target;
using System;

namespace SQGame.Logic.Behaviour
{
    public class BehaviourFactory
    {
        // [Initialization]
        // ****************************************************************************************************
        public BehaviourFactory()
        {

        }

        // [Methods]
        // ****************************************************************************************************

        public IBehaviour Get(Behaviour type, ITargetable target)
        {
            switch (type)
            {
                case Behaviour.None:
                    return null;

                case Behaviour.Continue:
                    return new Continue();

                case Behaviour.PlayerController:
                    return new PlayerControllerInput();

                case Behaviour.ToTarget:
                    return new ToTarget(target);

                default:
                    throw new NotImplementedException($"{type} is not implemented.");
            }
        }

        public enum Behaviour
        {
            None,
            Continue,
            PlayerController,
            ToTarget
        }
    }

}