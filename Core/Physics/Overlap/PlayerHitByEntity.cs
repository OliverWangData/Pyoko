using SQGame.Singletons;
using SQGame.Entities;

namespace SQGame.Physics.Overlap
{
    public class PlayerHitByEntity : IOverlap
    {
        public void Entered(Entity player, Entity character)
        {
            player.Life -= GameCalculations.GetCollisionDamage(character.DataId, character.Tags, player.DataId, player.Tags);
        }
    }
}