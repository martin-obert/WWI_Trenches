using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    [CreateAssetMenu(menuName = "Character/Basic/Basic Aim Order", fileName = "Basic Aim Order")]
    public class AimOrder : Order
    {

        public AimOrder(string name) : base(name)
        {
        }

        public override void Execute(CharacterOrderArguments arguments)
        {

            var weapon = arguments.Inventory.MainWeapon;

            if (weapon == null)
            {
                Debug.LogError("Player has no main weapon");
                return;
            }

            if (arguments.CurrentTarget == null)
            {
                Debug.LogError("Player has no target");
                return;
            }

            arguments.Navigator.Stop();
            arguments.Navigator.Disable();
            arguments.Navigator.LookOn(arguments.CurrentTarget.GameObject.transform);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 1);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
        }
    }
}