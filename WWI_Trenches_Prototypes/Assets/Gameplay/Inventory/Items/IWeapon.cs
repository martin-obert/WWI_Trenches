using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public interface IWeapon : IItem
    {
        void MeleeAttack(ITargetable target, IIdentificable shooter);

        ITargetable Target { get; }
        
        bool IsInMeleeRange { get; }

        bool CanFire { get; }

        float AttackSpeed { get; }

        IIdentificable Owner { get; }

        WeaponData Data { get;  }
    }
}