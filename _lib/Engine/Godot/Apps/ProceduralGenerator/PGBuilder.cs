using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

using SQLib.Procedural;
using SQLib.Extensions;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class PGBuilder : Sampler
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public float Scale
        {
            get { return _scale; }
            set { _scale = value; PropertyValueChanged.Invoke(); }
        }
        private float _scale = 1;

        [Export] public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _offset;

        // ****************************************************************************************************

        [ExportGroup("Noise")]
        [Export] public NoiseSampler.BaseNoiseType Noise
        {
            get { return _noise; }
            set { _noise = value;  NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private NoiseSampler.BaseNoiseType _noise;

        [Export] public float NoiseScale
        {
            get { return _noiseScale; }
            set { _noiseScale = value; PropertyValueChanged.Invoke(); }
        }
        private float _noiseScale = 1;

        [Export] public Vector2 NoiseOffset
        {
            get { return _noiseOffset; }
            set { _noiseOffset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _noiseOffset;
        [Export] public float Seed
        {
            get { return _seed; }
            set { _seed = value; PropertyValueChanged.Invoke(); }
        }
        private float _seed;

        // ****************************************************************************************************
        [ExportGroup("Brownian Fractal")]
        [Export] public bool UseBrownian
        {
            get { return _useBrownian; } 
            set { _useBrownian = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private bool _useBrownian;

        [Export] public Vector2 BrownianOffset
        {
            get { return _brownianOffset; }
            set { _brownianOffset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _brownianOffset;

        [Export] public int Octaves
        {
            get { return _octaves; }
            set { _octaves = value; PropertyValueChanged.Invoke(); }
        }
        private int _octaves = 4;

        [Export] public float Persistance
        {
            get { return _persistance; }
            set { _persistance = value; PropertyValueChanged.Invoke(); }
        }
        private float _persistance = 0.5f;

        [Export] public float Lacunarity
        {
            get { return _lacunarity; }
            set { _lacunarity = value; PropertyValueChanged.Invoke(); }
        }
        private float _lacunarity = 2f;

        private static readonly string[] _BROWNIAN_PROPERTIES = new string[] { "Octaves", "Persistance", "Lacunarity", "BrownianOffset" };

        // ****************************************************************************************************
        [ExportGroup("Domain Warp")]
        [Export] public bool UseWarp
        {
            get { return _useWarp; }
            set { _useWarp = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private bool _useWarp;

        [Export] public Vector2 WarpOffset
        {
            get { return _warpOffset; }
            set { _warpOffset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _warpOffset;

        [Export] NoiseSampler.BaseNoiseType WarpNoise
        {
            get { return _warpNoise; }
            set { _warpNoise = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private NoiseSampler.BaseNoiseType _warpNoise;

        [Export] public int Layers
        {
            get { return _layers; }
            set { _layers = value; PropertyValueChanged.Invoke(); }
        }
        private int _layers = 1;

        [Export] public float Strength
        {
            get { return _strength; }
            set { _strength = value; PropertyValueChanged.Invoke(); }
        }
        private float _strength = 1;


        [ExportSubgroup("Shift")]
        [Export] public float WarpShiftScale
        {
            get { return _warpShiftScale; }
            set { _warpShiftScale = value; PropertyValueChanged.Invoke(); }
        }
        private float _warpShiftScale = 1;

        [Export] public Vector2 WarpShiftOffset
        {
            get { return _warpShiftOffset; }
            set { _warpShiftOffset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _warpShiftOffset;

        [Export] public float WarpShiftSeed
        {
            get { return _warpShiftSeed; }
            set { _warpShiftSeed = value; PropertyValueChanged.Invoke(); }
        }
        private float _warpShiftSeed;

        [Export] public bool UseBrownianOnShift
        {
            get { return _useBrownianOnShift; }
            set { _useBrownianOnShift = value; NotifyPropertyListChanged(); PropertyValueChanged.Invoke(); }
        }
        private bool _useBrownianOnShift;

        [Export] public Vector2 WarpBrownianOffset
        {
            get { return _warpBrownianOffset; }
            set { _warpBrownianOffset = value; PropertyValueChanged.Invoke(); }
        }
        private Vector2 _warpBrownianOffset;

        [Export] public int WarpShiftOctaves
        {
            get { return _warpShiftOctaves; }
            set { _warpShiftOctaves = value; PropertyValueChanged.Invoke(); }
        }
        private int _warpShiftOctaves = 4;

        [Export] public float WarpShiftPersistance
        {
            get { return _warpShiftPersistance; }
            set { _warpShiftPersistance = value; PropertyValueChanged.Invoke(); }
        }
        private float _warpShiftPersistance = 0.5f;

        [Export] public float WarpShiftLacunarity
        {
            get { return _warpShiftLacunarity; }
            set { _warpShiftLacunarity = value; PropertyValueChanged.Invoke(); }
        }
        private float _warpShiftLacunarity = 2f;

        private static readonly string[] _WARP_BROWNIAN_PROPERTIES = new string[] { "WarpBrownianOffset", "WarpShiftOctaves", "WarpShiftPersistance", "WarpShiftLacunarity" };
        private static readonly string[] _WARP_PROPERTIES = _WARP_BROWNIAN_PROPERTIES.Union(new string[] { "WarpNoise", "Layers", "Strength", "WarpOffset", "WarpShiftSeed", "WarpShiftScale", "WarpShiftOffset", "UseBrownianOnShift" }).ToArray();

        // [Godot]
        // ****************************************************************************************************
        // Hide certain export fields based on conditions
        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            bool remove = false;
            string name = property["name"].As<string>();

            // Conditions to hide properties
            if (Noise == NoiseSampler.BaseNoiseType.Random && name.IsIn(new string[] { "Scale", "Offset" })) remove = true;     // Random doesn't use scale or offset
            if (!_useBrownian && name.IsIn(_BROWNIAN_PROPERTIES)) remove = true;                                                // Not using brownian so remove all brownian members
            if (!_useWarp && name.IsIn(_WARP_PROPERTIES)) remove = true;                                                        // Not using warp so remove all warp members
            else if (_useWarp && !UseBrownianOnShift && name.IsIn(_WARP_BROWNIAN_PROPERTIES)) remove = true;                    // Using warp, not using brownian so remove all warp-brownian members
            else if (                                                                                                           // Using warp, brownian is random so remove non-random members
                _useWarp && UseBrownianOnShift && WarpNoise == NoiseSampler.BaseNoiseType.Random && 
                name.IsIn(new string[] { "WarpShiftScale", "WarpShiftOffset" })
                ) remove = true;    

            // Hiding
            if (remove) property["usage"] = Variant.From(PropertyUsageFlags.NoEditor);
        }

        // [Methods]
        // ****************************************************************************************************
        public override float Sample(float x, float y)
        {
            Vector2 sample = new Vector2(x, y) / Scale + Offset;

            // Warp
            if (_useWarp)
            {
                return NoiseSampler.Warp(
                    Noise, WarpNoise, sample.X, sample.Y, 
                    WarpOffset.X, WarpOffset.Y, Layers, Strength,
                    UseBrownianOnShift, WarpBrownianOffset.X, WarpBrownianOffset.Y, WarpShiftOctaves, WarpShiftPersistance, WarpShiftLacunarity, 
                    WarpShiftScale, WarpShiftOffset.X, WarpShiftOffset.Y, WarpShiftSeed, 
                    _useBrownian, BrownianOffset.X, BrownianOffset.Y, Octaves, Persistance, Lacunarity,
                    _noiseScale, _noiseOffset.X, _noiseOffset.Y, Seed
                    );
            }

            // Brownian
            if (_useBrownian)
            {
                return NoiseSampler.Brownian(
                Noise, sample.X, sample.Y,
                BrownianOffset.X, BrownianOffset.Y, Octaves, Persistance, Lacunarity,
                _noiseScale, _noiseOffset.X, _noiseOffset.Y, Seed
                );
            }

            // Base
            switch (Noise)
            {
                case NoiseSampler.BaseNoiseType.Random: return NoiseSampler.Random(sample.X, sample.Y, Seed);
                case NoiseSampler.BaseNoiseType.Perlin: return NoiseSampler.Perlin(sample.X, sample.Y, _noiseScale, _noiseOffset.X, _noiseOffset.Y, Seed);
                case NoiseSampler.BaseNoiseType.CellularValue: return NoiseSampler.Cellular(sample.X, sample.Y, _noiseScale, _noiseOffset.X, _noiseOffset.Y, Seed).Value;
                case NoiseSampler.BaseNoiseType.CellularDistance: return NoiseSampler.Cellular(sample.X, sample.Y, _noiseScale, _noiseOffset.X, _noiseOffset.Y, Seed).DistCenter;
                default: return 0;
            }
        }
    }
}