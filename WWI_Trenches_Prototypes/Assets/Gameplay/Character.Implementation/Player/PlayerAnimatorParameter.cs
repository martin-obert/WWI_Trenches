using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public sealed class PlayerAnimatorParameter
    {
        public static readonly int BlendXHandle = Animator.StringToHash("BlendX");
        public static readonly int BlendYHandle = Animator.StringToHash("BlendY");
    }
}