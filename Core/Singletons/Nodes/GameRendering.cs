using Godot;
using SQLib;
using System;

namespace SQGame.Singletons
{
    public partial class GameRendering : Node
    {
        // [Fields]
        // ****************************************************************************************************
        public const float OFFSCREEN_PADDING_SINGLE_SIDE = 32f;

        /// Number of decimals in the position to keep for draw index Z-fighting.
        /// Assuming base 16x9 resolution of 480x270, and assuming the player scales the screen to a triple monitor setup of 1440x270, we have
        ///     (270 * 1000 * 1440) + (1440 * 1000) = 390Mil indices, which is within the 2.1bil limit.
        public const float SUBPIXEL_POSITION_DECIMALS = 10 ^ 3;

        public Camera2D Camera;
        public Window Window;

        // [Singleton]
        // ****************************************************************************************************
        public static GameRendering Instance
        {
            get { return _instance; }
        }
        private static GameRendering _instance;

        // [Godot]
        // ****************************************************************************************************
        public override void _Ready()
        {
            Camera = GetViewport().GetCamera2D();
            Window = GetWindow();
        }
        public override void _EnterTree()
        {
            _instance = this;
        }
        public override void _ExitTree()
        {
            _instance = null;
        }

        // [Properties]
        // ****************************************************************************************************
        public Vector2 ScreenPosition
        {
            get { return Camera.GetScreenCenterPosition(); }
        }

        public Vector2I WindowSize
        {
            get { return Window.ContentScaleSize; }
        }
        public Rect2 GameRect
        {
            get 
            {
                Vector2 padding = new Vector2(OFFSCREEN_PADDING_SINGLE_SIDE, OFFSCREEN_PADDING_SINGLE_SIDE);
                return new Rect2(ScreenPosition - (WindowSize / 2) - padding, WindowSize + (padding * 2)); 
            }
        }

        // [Methods]
        // ****************************************************************************************************
        /// We give each screen pixel a draw index, so we use ScreenSize.X * ScreenSize.Y indices. 
        /// To prevent Z-Fighting, we can also use subpixel position values by multiplying the position by SUBPIXEL_POSITION_DECIMALS before assigning the index.
        /// Max value of int is 2,147,483,647 , so we need to use under 2.1 billion draw indexes, otherwise the int will overflow and weird draw behaviour will happen.
        public int GetDrawIndex(Vector2 position) => (SQMath.IntFloor((position.Y - ScreenPosition.Y) * SUBPIXEL_POSITION_DECIMALS) * WindowSize.X) + SQMath.IntFloor(position.X * SUBPIXEL_POSITION_DECIMALS);
        public bool IsOutOfBounds(Vector2 position) => !GameRect.HasPoint(position);
        public Vector2 GetOcclusionWrap(Vector2 position)
        {
            Rect2 rect = GameRect;
            Vector2 temp = position;

            if (position.X < rect.Position.X) temp.X = rect.End.X;
            if (position.X > rect.End.X) temp.X = rect.Position.X;
            if (position.Y < rect.Position.Y) temp.Y = rect.End.Y;
            if (position.Y > rect.End.Y) temp.Y = rect.Position.Y;

            return temp;
        }
    }
}
