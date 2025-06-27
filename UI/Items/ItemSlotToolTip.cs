using Godot;
using SQGame.Rendering.Animations;
using SQGame.Singletons;
using SQGame.UI.Items;
using System;

namespace SQGame.UI
{
    public partial class ItemSlotToolTip : Control
    {
        // [Fields]
        // ****************************************************************************************************
        [ExportCategory("Dependencies")]
        [Export] private ItemSlot slot;
        [Export] private RichTextLabel name;
        [Export] private RichTextLabel description;

        const string STYLE_BBCODE = "[font=res://_lib/Fonts/FNT_LanaPixel_S11.ttf][font_size=11][center]";

        // [Methods]
        // ****************************************************************************************************
        public void SetData(ItemType type, int id)
        {
            slot.Initialize(type, id);

            switch (type)
            {
                case ItemType.Entity:
                    {
                        Data.Entities data = GameData.Instance.Get<int, Data.Entities>(id);
                        name.Text = Tr(data.LocName);
                        description.Text = Tr(data.LocDesc);
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