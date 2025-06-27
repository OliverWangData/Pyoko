using Godot;
using SQGame.Rendering.Animations;
using SQGame.Logic.Target;
using System;
using Godot.Collections;
using System.Diagnostics;

namespace SQGame.Rendering.Components
{
    public class AnimationPlayer : IRenderComponent, IDisposable
    {
        // [Fields]
        // ****************************************************************************************************
        public AnimationState CurrentAnimation { get; protected set; }
        public EntityTransform Transform { get; protected set; }
        public bool FlipCanvas;

        private readonly Rid Canvas;
        private readonly Texture2D Atlas;
        private readonly Dictionary<AnimationState, AnimationInfo> Animations;

        // [Initialization]
        // ****************************************************************************************************
        public AnimationPlayer(Rid parentCanvas, Texture2D atlas, AnimationInfo[] animations)
        {
            Canvas = RenderingServer.CanvasItemCreate();
            RenderingServer.CanvasItemSetParent(Canvas, parentCanvas);

            Atlas = atlas;

            Animations = new();
            for (int i = 0; i < animations.Length; i++)
            {
                Debug.Assert(!Animations.ContainsKey(animations[i].State), $"Duplicate state {animations[i].State} for AnimationPlayer.");
                Animations[animations[i].State] = animations[i];
            }

            // Mandatory animations
            Debug.Assert(Animations.ContainsKey(AnimationState.Default), $"Mandatory animations missing for AnimationPlayer.");
        }

        // [Finalization]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~AnimationPlayer() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            RenderingServer.FreeRid(Canvas);
            Disposed = true;
        }

        // [Methods - Rendering]
        // ****************************************************************************************************
        public void SetRenderTransform(EntityTransform transform)
        {
            Transform = transform;
            RenderingServer.CanvasItemSetTransform(Canvas, new(
                rotation: Transform.Rotation,
                scale: new Vector2(FlipCanvas ? -1 : 1, 1) * Transform.Scale,
                skew: 0,
                origin: Transform.Position
                ));
        }

        public void SetDrawIndex(int index) => RenderingServer.CanvasItemSetDrawIndex(Canvas, index);

        public void Play(AnimationState state)
        {
            RenderingServer.CanvasItemClear(Canvas);
            CurrentAnimation = state;
            AnimationInfo anim = Animations.ContainsKey(state) ? Animations[state] : Animations[AnimationState.Default];
            float animationDuration = anim.Positions.Length / anim.Fps;
            float frameDuration = 1 / anim.Fps;

            for (int i = 0; i < anim.Positions.Length; i++)
            {
                // Draws the animation textures onto the canvas item, with the anchor at the bottom-center of the texture.
                Vector2 offset = -new Vector2(anim.Size.X / 2, anim.Size.Y);

                RenderingServer.CanvasItemAddAnimationSlice(Canvas, animationDuration, frameDuration * i, frameDuration * (i + 1));
                Atlas.DrawRectRegion(Canvas, new Rect2(offset, anim.Size), new Rect2(anim.Positions[i], anim.Size));
            }
        }
    }
}