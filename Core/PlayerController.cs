using Godot;
using System;
using SQGame.Singletons;
using SQGame.Entities;
using SQGame.Logic.Behaviour;
using SQGame.Logic.Target;

namespace SQGame.Core
{
    public class PlayerController : IDisposable
    {
        public Entity Player { get; private set; }
        private PlayerControllerInput playerBehaviour;

        // [Initialization]
        // ****************************************************************************************************
        public PlayerController(EntityBuilder entityBuilder)
        {
            Player = entityBuilder.Build(0, new EntityTransform());
            playerBehaviour = new PlayerControllerInput();
            Player.Behaviours.Add(playerBehaviour, -1);

            InputEvents.Instance.Movement.Add(Move);
        }

        // [Finalization]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~PlayerController() => Dispose(false);

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
                InputEvents.Instance.Movement.Remove(Move);
                Player.Dispose();
            }

            Disposed = true;
        }

        // [Methods]
        // ****************************************************************************************************
        private void Move(Vector2 direction)
        {
            if (playerBehaviour is not null)
            {
                playerBehaviour.Direction = direction;
            }
        }

    }

}