using Godot;
using SQGame.Stats;
using SQGame.Entities;

namespace SQGame.Singletons
{
    public static class GameCalculations
    {
        public static float GetCollisionDamage(this int attackerId, EntityTag attackerTags, int defenderId, EntityTag defenderTags)
        {
            float attackerDamage = Modifiers.GetModifiedValue(
                GameData.Instance.Get<int, Data.Entities>(attackerId).Damage,
                GameStats.Instance.GetModifiers(StatType.Damage, attackerId, attackerTags)
                );

            float defenderDefense = Modifiers.GetModifiedValue(
                GameData.Instance.Get<int, Data.Entities>(defenderId).Defense,
                GameStats.Instance.GetModifiers(StatType.Defense, defenderId, defenderTags)
                );

            // Allows negative damage (healing) if the projectile has negative attack. 
            // But defense cannot apply "extra healing"
            if (attackerDamage > 0)
            {
                return Mathf.Max(attackerDamage - defenderDefense, 0);
            }
            else
            {
                return attackerDamage;
            }
        }
    }
}