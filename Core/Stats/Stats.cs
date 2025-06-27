using System;
using System.Collections.Generic;
using SQGame.Entities;
using SQGame.Singletons;

namespace SQGame.Stats
{
    public class Stats
    {
        // [Fields]
        // ****************************************************************************************************
        public const StatType PLAYER_STATS = StatType.Pickup | StatType.Growth | StatType.Revives;
        public const StatType ENTITY_STATS = ~PLAYER_STATS;

        public Dictionary<StatType, Modifiers> PlayerModifiers = new();
        public Dictionary<StatType, Dictionary<int, Modifiers>> EntityModifiers = new();
        public Dictionary<StatType, Dictionary<EntityTag, Modifiers>> EntityTagModifiers = new();

        // Caches a reference each time an Entity requests the modifiers
        // Advantage is no need to loop each time a stat modifier needs to be accessed
        // Disadvantage is slight increase to memory cost, and tags cannot be dynamically adjusted
        private static Dictionary<StatType, Dictionary<int, Modifiers[]>> entityModifiersCache = new(); 

        // [Godot]
        // ****************************************************************************************************
        public Stats()
        {
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                if (PLAYER_STATS.HasFlag(statType))
                {
                    PlayerModifiers[statType] = new();
                }
                else if (ENTITY_STATS.HasFlag(statType))
                {
                    EntityModifiers[statType] = new();
                    EntityTagModifiers[statType] = new();
                    entityModifiersCache[statType] = new();

                    foreach (int id in GameData.Instance.GetTable<int, Data.Entities>().Keys) EntityModifiers[statType].Add(id, new());
                    foreach (EntityTag tag in Enum.GetValues(typeof(EntityTag))) EntityTagModifiers[statType].Add(tag, new());
                }
            }
        }

        // [Methods]
        // ****************************************************************************************************
        public Modifiers[] GetModifiers(StatType statType, int entityId, EntityTag tags)
        {
            if (!entityModifiersCache[statType].ContainsKey(entityId))
            {

                List<Modifiers> modifiers = new()
                {
                    EntityModifiers[statType][entityId]
                };

                foreach (EntityTag entityTags in Enum.GetValues(typeof(EntityTag)))
                {
                    if (entityTags.HasFlag(tags))
                    {
                        modifiers.Add(EntityTagModifiers[statType][entityTags]);
                    }
                }

                entityModifiersCache[statType][entityId] = modifiers.ToArray();
            }


            return entityModifiersCache[statType][entityId];
        }
    }

}