using SQGame.Entities;
using SQGame.Singletons;
using System;

namespace SQGame.Physics.Overlap
{
    public class EntityOverlap
    {
        private EntityServer entityServer;
        private OverlapFactory overlapFactory;

        // [Constructors]
        // ****************************************************************************************************
        public EntityOverlap(EntityServer entityServer, OverlapFactory overlapFactory)
        {
            this.entityServer = entityServer;
            this.overlapFactory = overlapFactory;
        }

        // [Methods]
        // ****************************************************************************************************
        public void HandleOverlap(PhysicsOverlap physicsOverlap, int attackerPhysicsId, int defenderPhysicsId)
        {
            Entity attacker = entityServer.Find(attackerPhysicsId);
            Entity defender = entityServer.Find(defenderPhysicsId);

            // Skips collision if either attacker or defender have already been disposed
            if (attacker is null || defender is null || !attacker.IsActive || !defender.IsActive)
            {
                return;
            }

            IOverlap overlap = overlapFactory.Get(GameData.Instance.Get<int, Data.Entities>(attacker.DataId).Overlap);

            switch (physicsOverlap)
            {
                case PhysicsOverlap.Entered:
                    overlap?.Entered(attacker, defender);
                    break;

                case PhysicsOverlap.Colliding:
                    overlap?.Continuous(attacker, defender);
                    break;

                case PhysicsOverlap.Exited:
                    overlap?.Exited(attacker, defender);
                    break;

                default:
                    throw new NotImplementedException($"{physicsOverlap} not implemented.");
            }
        }
    }
}