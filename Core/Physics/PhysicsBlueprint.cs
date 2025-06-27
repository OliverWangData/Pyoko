using Godot;
using Godot.Collections;
using SQLib.Extensions;
using static SQGame.Physics.PhysicsComponent;

namespace SQGame.Physics
{
    [GlobalClass, Tool]
    public partial class PhysicsBlueprint : Resource
    {
        // [Fields]
        // ****************************************************************************************************
#if TOOLS
        [ExportCategory("Editor")]
        [Export(PropertyHint.MultilineText)] public string Notes;
#endif
        [Export] public PhysicsShape Shape
        {
            get { return _shape; }
            set { _shape = value; NotifyPropertyListChanged(); }
        }
        private PhysicsShape _shape;

        [Export] public float ShapeParameter1;
        [Export] public float ShapeParameter2;
        [Export] public Vector2 Offset;
        [Export(PropertyHint.Layers2DPhysics)] public byte CollisionLayers;
        [Export(PropertyHint.Layers2DPhysics)] public byte CollisionMasks;
        [Export] public bool enableEnterMonitor;
        [Export] public bool enableExitMonitor;
        [Export] public bool enableContinuousMonitor;
        [Export] public bool IsCollideable;

        public override void _ValidateProperty(Dictionary property)
        {
            base._ValidateProperty(property);

            string name = property["name"].As<string>();

            if (new string[] {"ShapeParameter1", "ShapeParameter2" }.Contains(name))
            {
                switch (Shape)
                {
                    case PhysicsShape.Circle:
                        {
                            if (name == "ShapeParameter1")
                            {
                                //property["name"] = "Radius";
                            }

                            else if (name == "ShapeParameter2")
                            {
                                property["usage"] = Variant.From(PropertyUsageFlags.NoEditor);
                            }
                        }
                        break;

                    case PhysicsShape.Rectangle:
                        {
                            if (name == "ShapeParameter1")
                            {
                                //property["name"] = "Width";
                            }

                            else if (name == "ShapeParameter2")
                            {
                                //property["name"] = "Height";
                            }
                        }
                        break;

                    case PhysicsShape.Cone:
                        {
                            if (name == "ShapeParameter1")
                            {
                                //property["name"] = "Radius";
                            }

                            else if (name == "ShapeParameter2")
                            {
                                //property["name"] = "Spread (Radian)";
                            }
                        }
                        break;
                }
            }
        }
    }
}