using Godot;
using System;
using SQGame.Singletons;
using SQGame.Logic.Target;

namespace SQGame.Logic.Spawn
{
    public class SpawnFactory
    {
        // [Initialization]
        // ****************************************************************************************************
        public SpawnFactory()
        {

        }

        // [Methods]
        // ****************************************************************************************************

        public ISpawn Get(Spawn type, ITargetable spawnOrigin, ITargetable target)
        {
            switch (type)
            {
                case Spawn.None:
                    return null;

                case Spawn.Point:
                    return new Point(spawnOrigin, target);

                case Spawn.EdgeOfScreen:
                    return new Box(spawnOrigin, target, GameRendering.Instance.GameRect.Size);

                case Spawn.ShotgunTight:
                    return new Cone(spawnOrigin, target, 45);

                case Spawn.ShotgunMedium:
                    return new Cone(spawnOrigin, target, 90);

                case Spawn.ShotgunLoose:
                    return new Cone(spawnOrigin, target, 135);

                case Spawn.Radial:
                    return new Radial(spawnOrigin, target);

                default:
                    throw new NotImplementedException($"{type} is not implemented.");
            }
        }

        public enum Spawn
        {
            None,
            Point,
            EdgeOfScreen,

            ShotgunTight,
            ShotgunMedium,
            ShotgunLoose,
            Radial
        }
    }

}