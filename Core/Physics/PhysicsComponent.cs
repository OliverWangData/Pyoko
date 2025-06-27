using Godot;
using System;
using System.Runtime.InteropServices;

namespace SQGame.Physics
{
    [StructLayout(LayoutKind.Explicit, Size = COMPONENT_SIZE)]
    public struct PhysicsComponent
    {
        // [Fields]
        // ****************************************************************************************************
        public const int COMPONENT_SIZE = sizeof(int) * 2 + sizeof(byte) * 4 + sizeof(float) * 12;

        [FieldOffset(0)]
        public int Id;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 0 + sizeof(float) * 0)]
        public PhysicsFlags Flags;

        /// <summary>
        /// 8 Distince collision layers/masks.
        /// 0000 0001 -> Players
        /// 0000 0010 -> Player projectiles
        /// 0000 0100 -> Enemies
        /// 0000 1000 -> Enemy projectiles
        /// </summary>
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 1 + sizeof(float) * 0)]
        public byte Layer;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 2 + sizeof(float) * 0)]
        public byte Mask;


        /// <summary>
        /// </summary>
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 3 + sizeof(float) * 0)]
        public PhysicsShape Shape;
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 0)]
        public float ShapeParameter1;
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 1)]
        public float ShapeParameter2;


        // Physics will not stop components from rotating or scaling
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 2)]
        public float Rotation;

        // Current position of the component
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 4)]
        public float PositionX;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 5)]
        public float PositionY;

        // Target position of the component after the physics process
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 6)]
        public float CollisionOffsetX;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 7)]
        public float CollisionOffsetY;

        // Min and Max X position (For sorting and sweeping)
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 8)]
        public float LeftBound;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 9)]
        public float RightBound;
        // Min and Max X position (For sorting and sweeping)
        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 10)]
        public float BottomBound;

        [FieldOffset(sizeof(int) * 1 + sizeof(byte) * 4 + sizeof(float) * 11)]
        public float TopBound;

        // [Enums]
        // ****************************************************************************************************
        /// IsActive -> True means that the struct is representing an entity. False means igore the entity.
        /// IsCollideable -> True enables being pushed by collision. False disables collision.
        /// IsMonitoringEntered -> True enables callbacks for "entered" overlap, happens when the physics shape collides with a new shape this frame. 
        /// IsMonitoringExited -> True enables callbacks for "exited" overlap, happens when the physics shape is no longer colliding with a shape it collided with the past frame. 
        /// IsMonitoringContinuous -> True enables callbacks for overlap on each frame while the overlap is happening.
        [Flags]
        public enum PhysicsFlags : byte
        {
            IsActive = 1 << 0,
            IsCollideable = 1 << 1,
            IsMonitoringEntered = 1 << 2,
            IsMonitoringExited = 1 << 3,
            IsMonitoringContinuous = 1 << 4
        }

        /// Circle
        ///     - P1: Radius
        ///     - P2: null
        /// Rectangle
        ///     - P1: Width
        ///     - P2: Height
        /// Cone
        ///     - P1: Radius
        ///     - P2: Spread
        public enum PhysicsShape : byte
        {
            Circle,
            Rectangle,
            Cone
        }

        // [Enums]
        // ****************************************************************************************************
        public unsafe static void CalculateBounds(PhysicsComponent* component)
        {
            unsafe
            {
                switch (component->Shape)
                {
                    case PhysicsShape.Circle:
                    default:
                        {
                            component->LeftBound = component->PositionX - component->ShapeParameter1;
                            component->RightBound = component->PositionX + component->ShapeParameter1;
                            component->BottomBound = component->PositionY - component->ShapeParameter1;
                            component->TopBound = component->PositionY + component->ShapeParameter1;
                            break;
                        }


                    case PhysicsShape.Rectangle:
                        {
                            // We rotate both corner-to-corner hypotenuses, which will give us two different x-axis lengths.
                            // We then center the larger one to get the left/right bounds.
                            // Same for the top-bottom bounds

                            float sin = MathF.Sin(component->Rotation);
                            float cos = MathF.Cos(component->Rotation);
                            float forwardX = MathF.Abs((cos * component->ShapeParameter1) - (sin * component->ShapeParameter2));
                            float forwardY = MathF.Abs((sin * component->ShapeParameter1) + (cos * component->ShapeParameter2));
                            float backwardX = MathF.Abs((cos * -component->ShapeParameter1) - (sin * component->ShapeParameter2));
                            float backwardY = MathF.Abs((sin * -component->ShapeParameter1) + (cos * component->ShapeParameter2));
                            float distX = (forwardX > backwardX ? forwardX : backwardX) / 2;
                            float distY = (forwardY > backwardY ? forwardY : backwardY) / 2;
                            component->LeftBound = component->PositionX - distX;
                            component->RightBound = component->PositionX + distX;
                            component->BottomBound = component->PositionY - distY;
                            component->TopBound = component->PositionY + distY;
                            break;
                        }


                    case PhysicsShape.Cone:
                        {
                            // For each bound there are 2 cases:
                            // 1: rotation is within the spread angle. Then the bound will be the radius.
                            // 2: rotation is not within the spread angle. Then the bound is the closest of the two corners and the center. 

                            // Left
                            float halfAngle = component->ShapeParameter2 / 2;
                            float counterclockX = MathF.Cos(component->Rotation + halfAngle) * component->ShapeParameter1;
                            float counterclockY = MathF.Sin(component->Rotation + halfAngle) * component->ShapeParameter1;
                            float clockX = MathF.Cos(component->Rotation - halfAngle) * component->ShapeParameter1;
                            float clockY = MathF.Sin(component->Rotation - halfAngle) * component->ShapeParameter1;

                            bool leftAligned = 3.1415926535f - halfAngle < component->Rotation && component->Rotation < 3.1415926535f + halfAngle;
                            bool rightAligned = 6.28318530718f - halfAngle < component->Rotation || component->Rotation < halfAngle;
                            bool bottomAligned = 4.71238898038f - halfAngle < component->Rotation && component->Rotation < 4.71238898038f + halfAngle;
                            bool topAligned = 1.57079632679f - halfAngle < component->Rotation && component->Rotation < 1.57079632679f + halfAngle;

                            component->LeftBound = leftAligned ? (component->PositionX - component->ShapeParameter1) : MathF.Min(MathF.Min(counterclockX, clockX), component->PositionX);
                            component->RightBound = rightAligned ? (component->PositionX + component->ShapeParameter1) : MathF.Max(MathF.Max(counterclockX, clockX), component->PositionX);
                            component->BottomBound = bottomAligned ? (component->PositionY - component->ShapeParameter1) : MathF.Min(MathF.Min(counterclockY, clockY), component->PositionY);
                            component->TopBound = topAligned ? (component->PositionY + component->ShapeParameter1) : MathF.Max(MathF.Max(counterclockY, clockY), component->PositionY);
                            break;
                        }

                }
            }
        }
    }
}