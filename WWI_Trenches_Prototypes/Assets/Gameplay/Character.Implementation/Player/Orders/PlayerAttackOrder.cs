using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    
    //public class WeaponSystem : Singleton<WeaponSystem>
    //{
    //    private void Awake()
    //    {
    //        CreateSingleton(this);
    //    }

    //    private void OnDestroy()
    //    {
    //        GCSingleton(this);
    //    }

    //    public void ActivateWeapon(IWeapon weapon)
    //    {
    //        Invo
    //    }
    //}

    public class PlayerAttackOrder : PlayerOrder
    {
        //private WeaponSystem _weaponSystem;

        public PlayerAttackOrder(string name) : base(name)
        {
            //_weaponSystem = InjectService.Instance.GetInstance<WeaponSystem>(system => _weaponSystem = system);
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

            if (!weapon.Target)
            {
                Debug.LogError("Weapon has no target");
                return;
            }

            if (!weapon.IsInRange)
            {
                arguments.Navigator.Move(weapon.Target.transform.position);

                weapon.IsInRangeChanged += WeaponOnIsInRangeChanged;
            }
            else
            {
                weapon.StartFiring();
            }

        }

        private void WeaponOnIsInRangeChanged(object sender, IWeapon weapon)
        {
            if (weapon.IsInRange)
            {
                weapon.StartFiring();
            }

            weapon.IsInRangeChanged -= WeaponOnIsInRangeChanged;
        }

    }
}