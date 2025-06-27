using SQGame.Entities;
using SQGame.Singletons;
using System;

namespace SQGame.Physics.Overlap
{
    public class OverlapFactory
    {
        // [Constructors]
        // ****************************************************************************************************
        public OverlapFactory()
        {
        }

        // [Methods]
        // ****************************************************************************************************
        public IOverlap Get(Overlap type)
        {
            switch (type)
            {
                case Overlap.None:
                    return null;

                case Overlap.ProjectileHitCharacter:
                    return new ProjectileHitCharacter();

                case Overlap.PlayerHitByEntity:
                    return new PlayerHitByEntity();

                default:
                    throw new NotImplementedException($"{type} not implemented.");
            }
        }

        // [Enums]
        // ****************************************************************************************************
        public enum Overlap
        {
            None,
            ProjectileHitCharacter,
            PlayerHitByEntity
        }
    }
}