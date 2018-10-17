using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerCoverOrder : PlayerOrder
    {
        public PlayerCoverOrder(string name) : base(name)
        {
        }


        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Navigator.Stop();
            arguments.Navigator.Disable();
            arguments.Navigator.LookAtDirection(Quaternion.Euler(0,180,0));
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, -1);
        }
    }


    public class PlayerAimOrder : PlayerOrder
    {

        public PlayerAimOrder(string name) : base(name)
        {
        }

        public override void Execute(PlayerOrderArguments arguments)
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