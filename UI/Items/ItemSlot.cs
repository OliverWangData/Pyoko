using Godot;
using SQGame.Rendering.Animations;
using SQGame.Singletons;
using SQGame.UI.Items;
using System;

namespace SQGame.UI
{
    public partial class ItemSlot : Control
    {
        // [Fields]
        // ****************************************************************************************************


        [ExportCategory("Dependencies")]
        [Export] private TextureRect icon;

        // [Methods]
        // ****************************************************************************************************
        public void Initialize(ItemType type, int id)
        {
            switch (type)
            {
                case ItemType.Entity:
                    {
                        Data.Entities data = GameData.Instance.Get<int, Data.Entities>(id);
                        AnimationBlueprint animationBlueprint = GD.Load<AnimationBlueprint>(data.ResAnimationBlueprint);

                        AtlasTexture tex = icon.Texture as AtlasTexture;
                        tex.Atlas = GD.Load<Texture2D>(data.ResTexture);
                        tex.Region = new Rect2(animationBlueprint.Animations[0].Positions[0], animationBlueprint.Animations[0].Size);
                        return;
                    }

                /*
                case ItemType.Upgrade:
                    {
                        return;
                    }*/

                default:
                    throw new NotImplementedException($"{type} not implemented.");
            }
        }
    }
}