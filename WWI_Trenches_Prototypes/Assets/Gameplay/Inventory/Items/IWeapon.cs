using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public interface IWeapon : IItem, IEquipable
    {
        void MeleeAttack(ITargetable target);

        ITargetable Target { get; }
        
        bool IsInMeleeRange { get; }

        bool CanFire { get; }

        float AttackSpeed { get; }

        WeaponData Data { get;  }
    }
}