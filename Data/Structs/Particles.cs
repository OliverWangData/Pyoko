using Godot;

namespace SQGame.Data
{
    public struct Particles
    {
        public int Id;
        public string Name;

        // Particle Parameters
        public bool OneShot;
        public int Amount;
        public float Lifetime;
        public float Explosiveness;
        public float Randomness;
        public bool LocalCoordinates;
        public int SubEmitter;

        // Resources
        public string ResPPM
        {
            get { return "res://Particles/" + _resPPM + ".tres"; }
            set { _resPPM = value; }
        }
        private string _resPPM;

        public string ResTexture
        {
            get { return _resTexture + ".png"; }
            set { _resTexture = value; }
        }
        private string _resTexture;
    }
}