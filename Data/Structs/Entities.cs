using SQGame.Entities;
using SQGame.Physics.Overlap;
using System;
using System.Collections.Generic;

namespace SQGame.Data
{
    public struct Entities
    {
        public int Id;
        public string Name;
        public string LocName;
        public string LocDesc;
        public EntityType Type;
        public EntityTag Tags;
        public OverlapFactory.Overlap Overlap;

        // Stats
        public float Lifetime;
        public float Regen;
        public float Defense;
        public float Dodge;
        public float Speed;
        public float Size;
        public float Damage;
        public float Knockback;

        // Resources
        public string ResTexture
        {
            get { return $"res://Assets/{GetFolder()}/" + _resTexture + ".png"; }
            set { _resTexture = value; }
        }
        private string _resTexture;

        public string ResPhysicsBlueprint
        {
            get { return $"res://Assets/Blueprints/Physics/" + _resPhysicsBlueprint + ".tres"; }
            set { _resPhysicsBlueprint = value; }
        }
        private string _resPhysicsBlueprint;

        public string ResAnimationBlueprint
        {
            get { return $"res://Assets/Blueprints/Animation/" + _resAnimationBlueprint + ".tres"; }
            set { _resAnimationBlueprint = value; }
        }
        private string _resAnimationBlueprint;

        private string GetFolder()
        {
            switch (Type)
            {
                case EntityType.Player:
                case EntityType.Ally:
                case EntityType.Enemy:
                    return "Characters";

                case EntityType.AllyProjectile:
                case EntityType.EnemyProjectile:
                    return "Projectiles";

                default:
                    throw new NotImplementedException($"{Type} not implemented.");
            }
        }
    }
}