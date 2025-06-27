using Godot;
using System;
using SQGame.Singletons;
using SQGame.Stats;
using SQGame.Logic.Behaviour;
using SQGame.Logic.Target;
using SQGame.Rendering.Animations.Animator;
using SQGame.Rendering.Components;
using SQGame.Collections;
using SQLib.Events;

namespace SQGame.Entities
{
    public class Entity : ITargetable, IDisposable
    {
        // [Fields]
        // ****************************************************************************************************
        public readonly int DataId;
        public readonly int PhysicsId;
        public readonly EntityTag Tags;
        public TimerStack<IBehaviour> Behaviours { get; set; }
        public IAnimator Animator { get; set; }

        private readonly Vector2 physicsOffset;
        private readonly Rendering.Components.AnimationPlayer animationPlayer;
        private readonly IRenderComponent[] renderables;

        private bool wrapOnOcclude = false; // Determines behaviour when entity is outside the occlusion window. Either wraps it or kills it.
        
        // [Constructor]
        // ****************************************************************************************************
        public Entity(int dataId, int physicsId, Vector2 physicsOffset, EntityTag tags, EntityTransform transform, Rendering.Components.AnimationPlayer animationPlayer, IRenderComponent[] renderables = null, bool wrapOnOcclude = false)
        {
            DataId = dataId;
            Tags = tags;
            Transform = transform;
            PhysicsId = physicsId;
            this.physicsOffset = physicsOffset;
            this.animationPlayer = animationPlayer;
            this.renderables = (renderables is not null) ? renderables : new IRenderComponent[0];
            this.wrapOnOcclude = wrapOnOcclude;

            Behaviours = new();

            animationPlayer.Play(0);
        }

        // [Finalizer]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~Entity() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                GamePhysics.Instance.RemoveComponent(PhysicsId);
                animationPlayer.Dispose();
                for (int i = 0; i < renderables.Length; i++) renderables[i].Dispose();
                Behaviours = null;
                Animator = null;
                OnDeath = null;
            }

            Disposed = true;
        }

        // [Events]
        // ****************************************************************************************************
        public Callback OnDeath = new();

        // [Properties]
        // ****************************************************************************************************
        public float Life
        {
            get
            {
                ActiveCheck();
                return _life;
            }
            set
            {
                _life = Mathf.Clamp(value, 0,
                    Modifiers.GetModifiedValue(
                        GameData.Instance.Get<int, Data.Entities>(DataId).Lifetime,
                        GameStats.Instance.GetModifiers(StatType.Lifetime, DataId, Tags)
                        )
                    );

                if (_life == 0)
                {
                    Die();
                }
            }
        }
        private float _life;

        public EntityTransform Transform
        {
            get
            {
                ActiveCheck();
                return _transform;
            }
            protected set
            {
                Animator?.UpdateAnimation(animationPlayer, value, _transform);
                _transform = value;
            }
        }
        private EntityTransform _transform = new(); // Needs to declare default constructor here so that Scale defaults to (1, 1) instead of (0, 0)

        public bool IsActive { get { return !Disposed; } }
        // [Methods]
        // ****************************************************************************************************
        public void ProcessPrePhysics(double delta)
        {
            ActiveCheck();

            IBehaviour behaviour = Behaviours.Peek();
            if (behaviour is not null)
            {
                // Gets the entity's direction from the behaviour. The entity sets the position itself since the entity knows its own speed
                EntityTransform transform = behaviour.Process(Transform, delta);
                transform.Position = transform.Position + (Transform.Direction * (float)delta * 
                    Modifiers.GetModifiedValue(
                        GameData.Instance.Get<int, Data.Entities>(DataId).Speed,
                        GameStats.Instance.GetModifiers(StatType.Speed, DataId, Tags)
                        )
                    );

                Transform = transform;
            }

            // Updates the physics object's transform
            //GamePhysics.Instance.SetTransform(PhysicsId, Transform.Position, Transform.Rotation, Transform.Scale);
            GamePhysics.Instance.SetPosition(PhysicsId, Transform.Position.X + physicsOffset.X, Transform.Position.Y + physicsOffset.Y);
            GamePhysics.Instance.SetRotation(PhysicsId, Transform.Rotation);
        }

        public void ProcessPostPhysics()
        {
            ActiveCheck();

            // Retrieves the physics object's transform after taking into account collisions, etc...
            EntityTransform transform = Transform;
            transform.Position = GamePhysics.Instance.GetPosition(PhysicsId) - physicsOffset;
            Transform = transform;

            // Updates all the render objects with the new transfrom from the physics object
            animationPlayer.SetRenderTransform(Transform);
            animationPlayer.SetDrawIndex(GameRendering.Instance.GetDrawIndex(Transform.Position));

            for (int i = 0; i < renderables.Length; i++)
            {
                renderables[i].SetRenderTransform(Transform);
                renderables[i].SetDrawIndex(GameRendering.Instance.GetDrawIndex(Transform.Position));
            }

            // Occlusion behaviour. Either wrap around (Enemies) or kill the entity (Projectiles)
            // Has to be called here as Die() may end up disposing the entity
            if (GameRendering.Instance.IsOutOfBounds(Transform.Position))
            {
                if (wrapOnOcclude)
                {
                    EntityTransform updatedTransfrom = Transform;
                    updatedTransfrom.Position = GameRendering.Instance.GetOcclusionWrap(Transform.Position);
                    Transform = updatedTransfrom;
                }
                else
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            ActiveCheck();

            OnDeath.Invoke();
            Dispose();
        }

        private void ActiveCheck()
        {
            if (!IsActive) throw new NullReferenceException("Entity has already been disposed.");
        }
    }
}