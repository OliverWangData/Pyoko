using Godot;

namespace SQGame.Singletons
{
    public static class GameStats
    {
        // [Field]
        // ****************************************************************************************************
        public static Stats.Stats Instance;

        // [Constructor]
        // ****************************************************************************************************
        static GameStats()
        {
            Instance = new();
        }
    }
}