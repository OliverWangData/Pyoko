using SQGame.Entities;

namespace SQGame.Physics.Overlap
{
    public interface IOverlap
    {
        public void Entered(Entity attacker, Entity defender) { }
        public void Continuous(Entity attacker, Entity defender) { }
        public void Exited(Entity attacker, Entity defender) { }
    }
}