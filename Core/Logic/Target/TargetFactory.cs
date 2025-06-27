using Godot;
using SQGame.Core;
using SQGame.Entities;
using System;

namespace SQGame.Logic.Target
{
    public class TargetFactory
    {
        private readonly FollowTargetableCamera2D targetableCamera2D;
        private readonly EntityServer entityServer;
        private ITargetable player;

        // [Initialization]
        // ****************************************************************************************************
        public TargetFactory(FollowTargetableCamera2D targetableCamera2D, EntityServer entityServer)
        {
            this.targetableCamera2D = targetableCamera2D;
            this.entityServer = entityServer;
        }

        public void SetPlayer(ITargetable player)
        {
            this.player = player;
        }

        // [Methods]
        // ****************************************************************************************************

        public ITargetable Get(Target type)
        {
            switch (type)
            {
                case Target.None:
                    return null;

                case Target.Camera:
                    return targetableCamera2D;

                case Target.Player:
                    return player;

                case Target.ClosestEnemyToPlayer:
                    return entityServer.FindClosestToPosition(player.Transform.Position, EntityType.Enemy);

                default:
                    throw new NotImplementedException($"{type} is not implemented.");
            }
        }

        public enum Target
        {
            None,
            Camera,
            Player,
            ClosestEnemyToPlayer
        }
    }

}