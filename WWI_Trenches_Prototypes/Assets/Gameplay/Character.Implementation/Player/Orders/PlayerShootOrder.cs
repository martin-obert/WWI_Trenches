using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerCoverOrder : PlayerOrder
    {
        public PlayerCoverOrder(string name) : base(name)
        {
        }

        protected override void Activate(PlayerOrderArguments arguments)
        {
            
        }

        public override void Deactivate(PlayerOrderArguments arguments)
        {
            arguments.Navigator.Enable();
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


    public class PlayerShootOrder : PlayerOrder
    {

        public PlayerShootOrder(string name) : base(name)
        {
        }

        protected override void Activate(PlayerOrderArguments arguments)
        {
            arguments.Navigator.Stop();
            
        }

        public override void Deactivate(PlayerOrderArguments arguments)
        {
            arguments.Inventory?.MainWeapon?.StopFiring();
        }

        public override void Execute(PlayerOrderArguments arguments)
        {

            var weapon = arguments.Inventory.MainWeapon;

            if (weapon == null)
            {
                Debug.LogError("Player has no main weapon");
                return;
            }

            if (arguments.Enemy == null)
            {
                Debug.LogError("Player has no target");
                return;
            }

            arguments.Navigator.LookOn(arguments.Enemy.GameObject.transform);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 1);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
            weapon.StartFiring(arguments.Enemy.GameObject.transform.position);
        }
    }
}