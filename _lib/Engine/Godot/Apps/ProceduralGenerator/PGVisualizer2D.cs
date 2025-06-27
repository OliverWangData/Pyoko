using Godot;
using SQLib.Extensions;
using SQLib.Procedural;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SQLib.GDEngine.ProceduralGenerator
{
    [GlobalClass, Tool]
    public partial class PGVisualizer2D : Sprite2D
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public Vector2I Size
        {
            get { return _size; }
            set { _size = value; UpdateTexture(); }
        }
        private Vector2I _size = new Vector2I(64, 64);

        [Export] public float ScaleOverride
        {
            get { return _scaleOverride; }
            set { _scaleOverride = value; UpdateTexture(); }
        }
        private float _scaleOverride = 1;

        [Export] public bool UseColorRamp
        {
            get { return _useColorRamp; }
            set { _useColorRamp = value; NotifyPropertyListChanged(); UpdateTexture(); }
        }
        private bool _useColorRamp;

        [Export] public Gradient ColorRamp
        {
            get { return _colorRamp; }
            set { _colorRamp = value; UpdateTexture(); }
        }
        private Gradient _colorRamp;

        [Export] public Sampler Sampler
        {
            get { return _sampler; }
            set
            {
                _sampler?.PropertyValueChanged.Remove(UpdateTexture);
                _sampler = value;
                _sampler?.PropertyValueChanged.Add(UpdateTexture);
                UpdateTexture();
            }
        }
        private Sampler _sampler;

        // ****************************************************************************************************
        // Multithreading
        [Export] private bool _isTaskOngoing;
        [Export] private bool _runTaskAgain;

        [Export] public bool SaveToPng
        {
            get { return false; }
            set { if (value) SaveTexture(); }
        }

        // [Godot]
        // ****************************************************************************************************
        // Hide certain export fields based on conditions
        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            bool remove = false;
            string name = property["name"].As<string>();

            if (!_useColorRamp && name == "ColorRamp") remove = true;
            if (remove) property["usage"] = Variant.From(PropertyUsageFlags.NoEditor);
        }

        // [Methods]
        // ****************************************************************************************************
        private void UpdateTexture()
        {
#if TOOLS
            if (Sampler is null) return;

            if (_isTaskOngoing)
            {
                _runTaskAgain = true;
            }
            else
            {
                _runTaskAgain = false;
                _isTaskOngoing = true;

                Task.Run(() =>
                {
                    ImageTexture display = Sampler.GetTexture(Size.X, Size.Y, 0, 0, ScaleOverride, ColorRamp);

                    // Updates existing ImageTexture asset, or creates a new one if none exist
                    CallDeferred(Sprite2D.MethodName.SetTexture, display);

                    // Starts generating again if a task has been queued
                    _isTaskOngoing = false;
                    if (_runTaskAgain) UpdateTexture();
                });
            }
#endif
        }

        private void SaveTexture()
        {
            Texture.GetImage().SavePng("res://VisualizerTexture.png");
            GD.Print("Saved texture to res://VisualizerTexture.png");
        }
    }
}
