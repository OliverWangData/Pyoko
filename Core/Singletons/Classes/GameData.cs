using Godot;
using SQGame.Data;

namespace SQGame.Singletons
{
    public static class GameData
    {
        // [Field]
        // ****************************************************************************************************
        public static Database Instance;

        // [Constructor]
        // ****************************************************************************************************
        static GameData()
        {
            Instance = new();
        }
    }
}