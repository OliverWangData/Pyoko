using Godot;
using System.Collections.Generic;
using SQGame.Singletons;
using SQGame.Core;
using SQGame.Entities;
using SQGame.Logic.Behaviour;
using SQGame.Logic.Spawn;
using SQGame.Logic.Target;
using SQGame.Physics.Overlap;
using SQGame.Rendering.Animations.Animator;

namespace SQGame.Bootstrap
{
    public partial class Game : Node2D
    {
        // [Fields]
        // ****************************************************************************************************

        // Environment
        private Rid space;
        private Rid canvas;
        private FollowTargetableCamera2D camera;
        private EntityServer entityServer;
        private EntityOverlap entityOverlap;

        // Factories
        private AnimatorFactory animatorFactory;
        private BehaviourFactory behaviourFactory;
        private SpawnFactory spawnFactory;
        private OverlapFactory overlapFactory;
        private TargetFactory targetFactory;

        // Entity
        private EntityBuilder entityBuilder;

        // Player
        private PlayerController playerController;


        private List<Entity> TestEntities = new();
        private double TestSpawnTimer = 0;
        private Vector2 testLine;

        // [Godot]
        // ****************************************************************************************************
        public override void _Ready()
        {
            // Environment
            space = GetWorld2D().Space;
            canvas = GetCanvasItem();
            camera = (FollowTargetableCamera2D)GetViewport().GetCamera2D();
            entityServer = new();

            // Factories
            spawnFactory = new();
            behaviourFactory = new();
            animatorFactory = new();
            overlapFactory = new();
            targetFactory = new(camera, entityServer);

            // Entity
            entityBuilder = new(canvas, spawnFactory, behaviourFactory, targetFactory, animatorFactory, entityServer);
            entityOverlap = new(entityServer, overlapFactory);
            GamePhysics.Instance.OverlapHandler = entityOverlap.HandleOverlap;

            // Player
            playerController = new(entityBuilder);
            targetFactory.SetPlayer(playerController.Player);
            camera.Target = playerController.Player;


            //TestSpawnEnemies(2);
            //TestSpawnProjectiles(1);
        }

        public override void _Process(double delta)
        {
            TestSpawnTimer += delta;

            if (TestSpawnTimer > 1)
            {
                TestSpawnTimer = 0;
                //TestSpawnAlly(1, 61);
                TestSpawnEnemies(100);
                TestSpawnProjectiles(24);

                GD.Print($"Count: {entityServer.Count}     FPS: {Engine.GetFramesPerSecond()}");
            }

            entityServer.Process(delta);
            
        }


        private void TestSpawnAlly(int count, int id)
        {
            Entity[] entities = entityBuilder.Build(

                spawnType: SpawnFactory.Spawn.Point,
                spawnOriginTargetType: TargetFactory.Target.Player,
                spawnTargetTargetType: TargetFactory.Target.Player,

                behaviourType: BehaviourFactory.Behaviour.ToTarget,
                behaviourTargetTargetType: TargetFactory.Target.Player,

                entityId: id,
                count: count
                );

            TestEntities.AddRange(entities);
        }

        private void TestSpawnEnemies(int count)
        {
            Entity[] entities = entityBuilder.Build(

                spawnType: SpawnFactory.Spawn.EdgeOfScreen, 
                spawnOriginTargetType: TargetFactory.Target.Camera, 
                spawnTargetTargetType: TargetFactory.Target.Player, 

                behaviourType: BehaviourFactory.Behaviour.ToTarget, 
                behaviourTargetTargetType: TargetFactory.Target.Player, 

                entityId: 100000,
                count: count
                );

            TestEntities.AddRange(entities);
        }

        private void TestSpawnProjectiles(int count)
        {
            Entity[] entities = entityBuilder.Build(

                spawnType: SpawnFactory.Spawn.ShotgunTight,
                spawnOriginTargetType: TargetFactory.Target.Player,
                spawnTargetTargetType: TargetFactory.Target.ClosestEnemyToPlayer,

                behaviourType: BehaviourFactory.Behaviour.Continue,
                behaviourTargetTargetType: default,

                entityId: 200000,
                count: count
                );

            QueueRedraw();
            TestEntities.AddRange(entities);
        }


        public override void _Draw()
        {
            return;
            base._Draw();
            if (playerController.Player is not null & targetFactory.Get(TargetFactory.Target.ClosestEnemyToPlayer) is not null)
            {
                DrawLine(playerController.Player.Transform.Position, targetFactory.Get(TargetFactory.Target.ClosestEnemyToPlayer).Transform.Position, Colors.Red, 1);
            }
            DrawRect(GameRendering.Instance.GameRect, Colors.Green, false, 4);
        }
    }
}