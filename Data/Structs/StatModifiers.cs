using Godot;
using SQGame.Stats;

namespace SQGame.Data
{
    public struct StatModifiers
    {
        public int Id;
        public string Name;

        // Particle Parameters
        public StatType Stat;
        public StatModifierType Type;
        public float Value;

        public string ResIcon
        {
            get { return _resIcon + ".png"; }
            set { _resIcon = value; }
        }
        private string _resIcon;
    }
}