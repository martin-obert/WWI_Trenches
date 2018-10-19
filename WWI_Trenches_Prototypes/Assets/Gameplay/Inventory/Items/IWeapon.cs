using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    public interface IWeapon : IItem
    {
        void FireOnce(Vector3 target, int shooterId);

        Vector3 Target { get; }
        
        bool IsInRange { get; }

        bool CanFire { get; }

        float AttackSpeed { get; }

        bool IsFiring { get; }

        int Id { get;  }

        int OwnerId { get; }

        WeaponData Data { get;  }

        Vector3 ProjectileSpawnLocation { get;  }
    }
}