using Godot;
using SQLib.Input;
using SQLib.Events;

namespace SQGame.Singletons
{
    /// <summary>
    /// Binds any game engine inputs into Callback delegates. Can also be used to create logical input groupings. 
    /// </summary>
    public partial class InputEvents : Node
    {
        // [Fields]
        // ****************************************************************************************************
        // Callbacks
        public Callback<Vector2> PointerPosition = new(), PointerPositionDelta = new(), Movement = new();
        public Callback<bool> Click = new(), KeyTab = new();

        // Internal input logic
        private DiscreteDoubleAxis movement = new();

        // [Singleton]
        // ****************************************************************************************************
        public static InputEvents Instance
        {
            get { return _instance; }
        }
        private static InputEvents _instance;

        // [Godot]
        // ****************************************************************************************************
        // Singletons using Autoload have to set their static instance in _EnterTree() rather than _Ready()
        // This is because _EnterTree() is called when first instancing, whereas _Ready() is called when child nodes in the scene tree have all been instanced.
        // Not doing so will cause null refs. 
        public override void _EnterTree() =>_instance = this;
        public override void _ExitTree() =>_instance = null;


        public override void _Input(InputEvent @event)
        {
            // Mouse movement
            if (@event is InputEventMouseMotion motionEvent)
            {
                PointerPosition.Invoke(motionEvent.Position);
                PointerPositionDelta.Invoke(motionEvent.Relative);
            }

            // Mouse Inputs
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    Click.Invoke(mouseEvent.Pressed);
                }
            }

            // Keyboard Inputs
            if (@event is InputEventKey keyEvent)
            {
                switch (keyEvent.Keycode)
                {
                    // WASD Movement
                    case Key.W:
                        if (movement.Up != keyEvent.Pressed)
                        {
                            movement.Up = keyEvent.Pressed;
                            Movement.Invoke(movement.Get());
                        }
                        break;

                    case Key.S:
                        if (movement.Down != keyEvent.Pressed)
                        {
                            movement.Down = keyEvent.Pressed;
                            Movement.Invoke(movement.Get());
                        }
                        break;

                    case Key.A:
                        if (movement.Left != keyEvent.Pressed)
                        {
                            movement.Left = keyEvent.Pressed;
                            Movement.Invoke(movement.Get());
                        }
                        break;

                    case Key.D:
                        if (movement.Right != keyEvent.Pressed)
                        {
                            movement.Right = keyEvent.Pressed;
                            Movement.Invoke(movement.Get());
                        }
                        break;

                    // Tab
                    case Key.Tab:
                        KeyTab.Invoke(keyEvent.Pressed);
                        break;
                }
            }
        }
    }
}
