using Godot;
using System;

namespace SQGame.Entities
{
    [Flags]
    public enum EntityType
    {
        Player = 1 << 0,
        Ally = 1 << 1,
        Enemy = 1 << 2,
        AllyProjectile = 1 << 3,
        EnemyProjectile = 1 << 4
    }
}