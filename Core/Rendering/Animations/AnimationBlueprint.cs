using Godot;

namespace SQGame.Rendering.Animations
{
    [GlobalClass]
    public partial class AnimationBlueprint : Resource
    {
        // [Fields]
        // ****************************************************************************************************
#if TOOLS
        [ExportCategory("Editor")]
        [Export(PropertyHint.MultilineText)] public string Notes;
#endif
        [Export] public AnimationInfo[] Animations;
    }
}