using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{

    public class PlayerShootOrder : PlayerOrder
    {

        public PlayerShootOrder(string name) : base(name)
        {
        }

        protected override void Activate(PlayerOrderArguments arguments)
        {

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

            weapon.StartFiring(arguments.Enemy.GameObject.transform.position);
        }
    }
}