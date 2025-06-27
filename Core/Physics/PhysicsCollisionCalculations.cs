using Godot;
using System;

public static class PhysicsCollisionCalculations
{
    public static Vector2 CircleCircleOffset(
            float positionAX, float positionAY, float radiusA, float rotationA,
            float positionBX, float positionBY, float radiusB, float rotationB
        )
    {

        Vector2 offset;

        float xDiff = positionAX - positionBX;
        float yDiff = positionAY - positionBY;
        float overlap = radiusA + radiusB - MathF.Sqrt(yDiff * yDiff + xDiff * xDiff);

        // Check to make sure it actually is overlapping
        if (overlap <= 0)
        {
            offset.X = 0;
            offset.Y = 0;
            return offset;
        }

        // Sohcahtoa for X and Y
        float angle = MathF.Atan2(yDiff, xDiff);
        offset.X = MathF.Cos(angle) * overlap;
        offset.Y = MathF.Sin(angle) * overlap;
        return offset;
    }
}
