using Godot;
using System;
using SQGame.Singletons;
using SQGame.Logic.Spawn;
using SQGame.Logic.Target;
using SQGame.Logic.Behaviour;
using SQGame.Rendering.Animations.Animator;
using SQGame.Physics;
using SQGame.Rendering.Animations;

namespace SQGame.Entities
{
    public class EntityBuilder
    {
        // [Fields]
        // ****************************************************************************************************
        // Required by the Builder
        private readonly Rid canvas;
        private readonly SpawnFactory spawnFactory;
        private readonly BehaviourFactory behaviourFactory;
        private readonly TargetFactory targetFactory;
        private readonly AnimatorFactory animatorFactory;
        private readonly EntityServer entityServer;

        // [Constructor]
        // ****************************************************************************************************
        public EntityBuilder(Rid canvas, SpawnFactory spawnFactory, BehaviourFactory behaviourFactory, TargetFactory targetFactory, AnimatorFactory animatorFactory, EntityServer entityServer)
        {
            this.canvas = canvas;
            this.spawnFactory = spawnFactory;
            this.behaviourFactory = behaviourFactory;
            this.targetFactory = targetFactory;
            this.animatorFactory = animatorFactory;
            this.entityServer = entityServer;
        }

        // [Methods]
        // ****************************************************************************************************
        // Uses an ISpawner to spawn 
        public Entity[] Build(
            SpawnFactory.Spawn spawnType, TargetFactory.Target spawnOriginTargetType, TargetFactory.Target spawnTargetTargetType, 
            BehaviourFactory.Behaviour behaviourType, TargetFactory.Target behaviourTargetTargetType, 
            int entityId, int count
            )
        {
            ISpawn spawn = spawnFactory.Get(spawnType, targetFactory.Get(spawnOriginTargetType), targetFactory.Get(spawnTargetTargetType));
            EntityTransform[] transforms = spawn.Get(count);
            Entity[] entities = Build(entityId, transforms);

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Behaviours.Add(behaviourFactory.Get(behaviourType, targetFactory.Get(behaviourTargetTargetType)), -1);
            }

            return entities;
        }

        public Entity Build(int id, EntityTransform transform)
        {
            Data.Entities data = GameData.Instance.Get<int, Data.Entities>(id);
            EntityType type = data.Type;
            Texture2D spriteSheet = GD.Load<Texture2D>(data.ResTexture);
            PhysicsBlueprint physicsBlueprint = GD.Load<PhysicsBlueprint>(data.ResPhysicsBlueprint);
            AnimationBlueprint animationBlueprint = GD.Load<AnimationBlueprint>(data.ResAnimationBlueprint);

            IAnimator animator;
            bool wrapOnOcclude;

            switch (type)
            {
                case EntityType.Player:
                case EntityType.Enemy:
                case EntityType.Ally:
                    animator = animatorFactory.Get(AnimatorFactory.Animator.Character);
                    wrapOnOcclude = true;
                    break;

                case EntityType.AllyProjectile:
                case EntityType.EnemyProjectile:
                    animator = animatorFactory.Get(AnimatorFactory.Animator.LeftRight);
                    wrapOnOcclude = false;
                    break;

                default:
                    throw new NotImplementedException($"{type} is not implemented.");
            }

            Rendering.Components.AnimationPlayer animationPlayer = new(canvas, spriteSheet, animationBlueprint.Animations);
            int physicsId = GamePhysics.Instance.AddComponent(physicsBlueprint, transform);
            Entity entity = new(id, physicsId, physicsBlueprint.Offset, data.Tags, transform, animationPlayer, wrapOnOcclude: wrapOnOcclude);
            entity.Animator = animator;
            entityServer.Add(entity, type);
            return entity;
        }

        public Entity[] Build(int id, EntityTransform[] transforms)
        {
            Entity[] entities = new Entity[transforms.Length];

            for (int i = 0; i < transforms.Length; i++)
            {
                entities[i] = Build(id, transforms[i]);
            }

            return entities;
        }
    }
}