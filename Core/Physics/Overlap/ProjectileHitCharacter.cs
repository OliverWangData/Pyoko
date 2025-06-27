using SQGame.Singletons;
using SQGame.Entities;

namespace SQGame.Physics.Overlap
{
    public class ProjectileHitCharacter : IOverlap
    {
        public void Entered(Entity projectile, Entity character)
        {
            character.Life -= GameCalculations.GetCollisionDamage(projectile.DataId, projectile.Tags, character.DataId, character.Tags);
            projectile.Life -= 1;
        }
    }
}