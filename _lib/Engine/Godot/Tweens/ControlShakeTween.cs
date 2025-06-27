using Godot;
using SQLib.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SQLib.GDEngine.Tweens
{
    public class ControlShakeTween : ITween
    {
        // [Fields]
        // ****************************************************************************************************
        public float Radius { get; private set; }
        public float Speed { get; private set; }
        public float Duration { get; private set; }

        private Tween tween;
        private Node target;
        private RandomNumberGenerator rng;
        private bool recursing;

        private Vector2 _startPosition;


        // [Initialization]
        // ****************************************************************************************************
        private ControlShakeTween(Node target, float radius, float speed, float duration)
        {
            this.target = target;
            Radius = radius;
            Speed = speed;
            Duration = duration;
            this.rng = new();
            Completed = new();
        }

        public ControlShakeTween(Node2D target, float radius, float speed, float duration) : this(target as Node, radius, speed, duration) { }
        public ControlShakeTween(Control target, float radius, float speed, float duration) : this(target as Node, radius, speed, duration) { }

        // [Events]
        // ****************************************************************************************************
        public Callback Completed { get; private set; }

        // [Properties]
        // ****************************************************************************************************
        private Vector2 CurrentPosition
        {
            get
            {
                if (typeof(Node2D).IsAssignableFrom(target.GetType()))
                {
                    return (target as Node2D).Position;
                }
                else
                {
                    return (target as Control).Position;
                }
            }
        }

        private Vector2 CurrentScale
        {
            get
            {
                if (typeof(Node2D).IsAssignableFrom(target.GetType()))
                {
                    return (target as Node2D).Scale;
                }
                else
                {
                    return (target as Control).Scale;
                }
            }
        }

        // [Methods]
        // ****************************************************************************************************
        public async void Play()
        {
            if (recursing) return;

            recursing = true;
            _startPosition = CurrentPosition;
            RecursiveShake();

            await Task.Delay((int)(Duration * 1000));

            recursing = false;
            Completed?.Invoke();
        }

        private void RecursiveShake()
        {
            // Generates tween parameter values randomly, with preference to outer values
            float angle = rng.RandfRange(-2 * MathF.PI, 2 * MathF.PI);
            Vector2 dist = new Vector2(rng.RandfRange(0, Radius * CurrentScale.X), rng.RandfRange(0, Radius * CurrentScale.Y));
            Vector2 position = new Vector2(dist.X * MathF.Cos(angle), dist.Y * MathF.Sin(angle));
            Vector2 targetPosition = recursing ? _startPosition + position : _startPosition;
            float duration = (targetPosition - CurrentPosition).Length() / Speed;

            tween = target.GetTree()?.CreateTween();
            tween.Parallel().TweenProperty(target, "position", targetPosition, duration);

            if (recursing)
            {
                tween.TweenCallback(Callable.From(RecursiveShake));
            }

            tween.Play();
        }
    }
}