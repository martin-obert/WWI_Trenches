using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    [CreateAssetMenu(menuName = "Character/Basic/Basic Crouch Order", fileName = "Basic Crouch Order")]
    public class CrouchOrder : Order
    {

        public CrouchOrder(string name) : base(name)
        {
        }

        public override void Execute(CharacterOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 1);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
        }
    }
}