using Godot;
using System;

namespace SQGame.Stats
{
    [Flags]
    public enum StatType
    {
        // Player-only
        Pickup = 1 << 0,
        Growth = 1 << 1,
        Revives = 1 << 2,

        // Lifetime
        Lifetime = 1 << 3,  // Equivalent to Max HP on characters, max pierce on projectiles
        Regen = 1 << 4,     // Recovers HP, or increases pierce over time
        Defense = 1 << 5,   // Reduces reduction on HP, or FMJ
        Dodge = 1 << 6,     // Avoids damage to HP, or avoid using up a pierce on projectile

        // Interactions
        Speed = 1 << 7,     // Movespeed / projectilespeed
        Size = 1 << 8,      // Scale
        Damage = 1 << 9,    // Can be damage applied to overlapping (Character->Player) (Projectile->Character)
        Knockback = 1 << 10 // Knockback applied to overlapping
    }
}