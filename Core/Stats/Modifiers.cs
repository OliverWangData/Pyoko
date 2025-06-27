using Godot;
using System;
using System.Collections.Generic;
using SQGame.Singletons;
using SQGame.Data;

namespace SQGame.Stats
{
    public class Modifiers
    {
        // [Fields]
        // ****************************************************************************************************
        public Dictionary<StatModifierType, List<int>> Mods = new(); // Keeps track of what Data.StatModifiers entries have been applied to the current modifier
        public Dictionary<StatModifierType, float> Values = new(); // Keeps track of the combined value of each type of stat modifier

        // [Initialization]
        // ****************************************************************************************************
        public Modifiers()
        {
            foreach (StatModifierType type in Enum.GetValues(typeof(StatModifierType)))
            {
                Mods[type] = new();
                Values[type] = GetDefaultModValue(type);
            }
        }

        // [Properties]
        // ****************************************************************************************************
        public float this[StatModifierType type]
        {
            get { return Values[type]; }
        }

        // [Methods]
        // ****************************************************************************************************
        public void Add(StatModifiers mod)
        {
            Mods[mod.Type].Add(mod.Id);
            Values[mod.Type] = AddModValue(Values[mod.Type], mod.Value, mod.Type);
        }
        public void Add(int id) => Add(GameData.Instance.Get<int, StatModifiers>(id));

        public bool Remove(StatModifiers mod)
        {
            if (Mods[mod.Type].Contains(mod.Id))
            {
                Values[mod.Type] = RemoveModValue(Values[mod.Type], mod.Value, mod.Type);
                return Mods[mod.Type].Remove(mod.Id);
            }

            return false;
        }

        public void Remove(int id) => Remove(GameData.Instance.Get<int, StatModifiers>(id));

        public float GetModifiedValue(float baseValue) => GetFinalValue(baseValue, Values);
        public static float GetModifiedValue(float baseValue, params Modifiers[] mods)
        {
            if (mods.Length == 0)
            {
                return 0;
            }

            Dictionary<StatModifierType, float> modValues = mods[0].Values;

            for (int i = 1; i < mods.Length; i++)
            {
                foreach (StatModifierType type in Enum.GetValues(typeof(StatModifierType)))
                {
                    modValues[type] = AddModValue(modValues[type], mods[i].Values[type], type);
                }
            }

            return GetFinalValue(baseValue, modValues);
        }


        private static float AddModValue(float value, float modValue, StatModifierType type)
        {
            switch (type)
            {
                case StatModifierType.Flat:
                case StatModifierType.PercentAdd:
                    return value + modValue;

                case StatModifierType.PercentMult:
                    return value * modValue;

                default:
#if TOOLS
                    throw new NotImplementedException();
#else
                    return value;
#endif

            }
        }

        private static float RemoveModValue(float value, float modValue, StatModifierType type)
        {
            switch (type)
            {
                case StatModifierType.Flat:
                case StatModifierType.PercentAdd:
                    return value - modValue;

                case StatModifierType.PercentMult:
                    return value / modValue;

                default:
                    throw new NotImplementedException();
            }
        }

        private static float GetDefaultModValue(StatModifierType type)
        {
            switch (type)
            {
                case StatModifierType.Flat:
                case StatModifierType.PercentAdd:
                    return 0;

                case StatModifierType.PercentMult:
                    return 1;

                default:
                    throw new NotImplementedException();

            }
        }

        private static float GetFinalValue(float baseValue, Dictionary<StatModifierType, float> modValues)
        {
            return (baseValue + modValues[StatModifierType.Flat]) * (1 + modValues[StatModifierType.PercentAdd]) * modValues[StatModifierType.PercentMult];
        }
    }
}