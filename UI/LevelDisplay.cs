using Godot;
using SQGame.Singletons;
using SQGame.World;
using System;

namespace SQGame.UI
{
    public partial class LevelDisplay : Control
    {
        // [Fields]
        // ****************************************************************************************************
        [ExportCategory("Dependencies")]
        [Export] private RichTextLabel nameLabel; 
        [Export] private string nameLabelBBCode = "[font=res://_lib/Fonts/FNT_LanaPixel_S11.ttf][font_size=11][center]";
        [Export] private PackedScene itemSlotPrefab;
        [Export] private Container effectsContainer;
        [Export] private Container enemiesContainer;
        [Export] private Container rewardsContainer;
        [Export] private ButtonGroup waypointGroup;

        // [Godot]
        // ****************************************************************************************************
        public override void _EnterTree()
        {
            base._EnterTree();
            waypointGroup.Pressed += HandleWaypointGroup;
            GD.Print(waypointGroup);
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            waypointGroup.Pressed -= HandleWaypointGroup;
        }

        // [Methods]
        // ****************************************************************************************************
        private void HandleFocusChanged()
        {
            // Using the "Pressed" signal on the ButtonGroup will always return the clicked button, whether that is because it has been selected or unselected.
            // Instead we want to check for the currently selected button in the group whenever something may have caused it to change. 
            BaseButton button = waypointGroup.GetPressedButton();

            // No waypoint selected
            if (button is null)
            {
                Toggle(false);
                return;
            }

            Waypoint waypoint = button as Waypoint;
            
            // The selected waypoint does not have an id
            if (waypoint.Id < 0)
            {
                GD.PushWarning("Waypoint has not been given an id.");
                Toggle(false);
                return;
            }

            Update(waypoint.Id);
        }

        private void HandleWaypointGroup(BaseButton button) => HandleFocusChanged();

        private void Update(int levelId)
        {

            Toggle(true);
        }

        private void Toggle(bool on)
        {
            if (on)
            {
                Visible = true;

            }
            else
            {

                Visible = false;
            }
        }
    }
}