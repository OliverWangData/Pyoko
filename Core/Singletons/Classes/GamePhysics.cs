using Godot;
using SQGame.Physics;

namespace SQGame.Singletons
{
    public static class GamePhysics
    {
        // [Field]
        // ****************************************************************************************************
        public static DiscreteSweepCollisionSystem Instance;

        // [Constructor]
        // ****************************************************************************************************
        static GamePhysics()
        {
            Instance = new DiscreteSweepCollisionSystem();
        }
    }
}