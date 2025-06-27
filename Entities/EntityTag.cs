using System;

namespace SQGame.Entities
{
    [Flags]
    public enum EntityTag
    {
        None,

        Animal,
        Aquatic,
        Monkey,
        Grazing,
        Chicken,

        Creature,
        Slime,


        Object,
        Magnet,

    }
}