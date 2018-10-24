using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    [CreateAssetMenu(menuName = "Items/Weapons",fileName = "BasicWeapon")]
    public class WeaponData : ScriptableObject
    {
        public string Name;

        public bool IsSingleHanded;

        [Tooltip("Attack frequency in seconds")]
        public float AttackSpeed = 0.5f;

        public float MeleeRange;
       
        public float FireRange;

        public int MaxStack;

        public int ClipMaxSize;

        public BasicProjectile Projectile;
    }
}