using Godot;
using SQLib.Events;
using System;

namespace SQGame.World
{
    public partial class Waypoint : TextureButton
    {
        // [Fields]
        // ****************************************************************************************************
        [Export] public int Id = -1;
        /*
        // [Godot]
        // ****************************************************************************************************
        public override void _EnterTree()
        {
            MouseEntered += HandleMouseEntered;
            MouseExited += HandleMouseExited;
        }


        public override void _ExitTree()
        {
            MouseEntered -= HandleMouseEntered;
            MouseExited -= HandleMouseExited;
        }

        // [Methods]
        // ****************************************************************************************************
        private void HandleMouseEntered()
        {
            ActiveWaypoint.Invoke(Id);
        }

        private void HandleMouseExited()
        {
            ActiveWaypoint.Invoke(-1);
        }*/
    }
}