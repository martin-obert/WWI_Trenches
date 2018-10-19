using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    [CreateAssetMenu(menuName = "Character/Basic/Cover order", fileName = "Cover order")]
    public class CoverOrder : Order
    {
        public CoverOrder(string name) : base(name)
        {
        }

        public override void Execute(CharacterOrderArguments arguments)
        {
            arguments.Navigator.Stop();
            arguments.Navigator.Disable();
            arguments.Navigator.LookAtDirection(Quaternion.Euler(0, 180, 0));
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, -1);
        }
    }
}