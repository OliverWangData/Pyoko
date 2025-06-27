using Godot;
using System;
using System.Collections.Generic;
using SQGame.Singletons;

namespace SQGame.Entities
{
    public class EntityServer : IDisposable
    {
        // [Fields]
        // ****************************************************************************************************
        private Dictionary<EntityType, List<Entity>> entities;
        private Dictionary<int, Entity> physicsIdCache; // Cached using physicsId

        // [Constructors]
        // ****************************************************************************************************
        public EntityServer()
        {
            entities = new();
            foreach (EntityType entityType in Enum.GetValues(typeof(EntityType))) entities[entityType] = new();
            physicsIdCache = new();
        }

        // [Finalizer]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~EntityServer() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                foreach (List<Entity> list in entities.Values)
                {
                    for (int i = 0; i < list.Count; i++) list[i].Dispose();
                }
            }

            Disposed = true;
        }

        // [Properties]
        // ****************************************************************************************************
        public int Count
        {
            get 
            {
                int count = 0;
                foreach (List<Entity> list in entities.Values) count += list.Count;
                return count; 
            }
        }

        // [Methods]
        // ****************************************************************************************************
        public void Add(Entity entity, EntityType entityType)
        {
            entities[entityType].Add(entity);
            physicsIdCache.Add(entity.PhysicsId, entity);
            entity.OnDeath.Add(() => Remove(entity));
        }

        public void Add(Entity[] entities, EntityType entityType)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Add(entities[i], entityType);
            }
        }

        public bool Remove(Entity entity)
        {
            foreach (EntityType entityType in Enum.GetValues(typeof(EntityType)))
            {
                if (entities[entityType].Contains(entity))
                {
                    entities[entityType].Remove(entity);
                    physicsIdCache.Remove(entity.PhysicsId);
                }
            }

            return false;
        }

        public Entity Find(int physicsId)
        {
            physicsIdCache.TryGetValue(physicsId, out Entity entity);
            return entity;
        }

        public Entity FindClosestToPosition(Vector2 position, EntityType entityType)
        {
            Entity closestEntity = default;
            float closestDistSquared = float.MaxValue;

            for (int i = 0; i < entities[entityType].Count; i++)
            {

                float distSquared = (position.X - entities[entityType][i].Transform.Position.X) * (position.X - entities[entityType][i].Transform.Position.X) + 
                    (position.Y - entities[entityType][i].Transform.Position.Y) * (position.Y - entities[entityType][i].Transform.Position.Y);

                if (distSquared < closestDistSquared)
                {
                    closestDistSquared = distSquared;
                    closestEntity = entities[entityType][i];
                }
            }

            return closestEntity;
        }

        public void Process(double delta)
        {
            foreach (List<Entity> entities in entities.Values)
            {
                for (int i = 0; i < entities.Count; i++) entities[i].ProcessPrePhysics(delta);
            }

            GamePhysics.Instance.Process();

            foreach (List<Entity> entities in entities.Values)
            {
                for (int i = 0; i < entities.Count; i++) entities[i].ProcessPostPhysics();
            }
        }
    }
}