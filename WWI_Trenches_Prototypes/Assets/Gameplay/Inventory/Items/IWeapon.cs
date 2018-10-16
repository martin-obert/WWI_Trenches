using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public interface IWeapon : IItem
    {
        void StartFiring(Vector3 target);

        void StopFiring();

        Vector3 Target { get; }
        
        bool IsInRange { get; }

        bool CanFire { get; }

        float AttackSpeed { get; }

        bool IsFiring { get; }
    }
}