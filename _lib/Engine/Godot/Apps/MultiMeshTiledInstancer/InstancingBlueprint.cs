using Godot;
using System;

namespace SQLib.GDEngine.MultiMeshTileInstancer
{
    [GlobalClass, Tool]
    public partial class InstancingBlueprint : Resource
    {
        [Export] public NodePath MultiMesh;
        [Export] public int TilemapLayer;
    }
}