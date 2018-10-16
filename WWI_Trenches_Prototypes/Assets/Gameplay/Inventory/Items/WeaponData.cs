using UnityEngine;

namespace Assets.Gameplay.Inventory.Items
{
    [CreateAssetMenu(menuName = "Items/Weapons",fileName = "BasicWeapon")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private float _range = 1f;

        public string Name;

        [Tooltip("Attack frequency in seconds")]
        public float AttackSpeed = 0.5f;

        public float Range
        {
            get { return _range; }
        }
    }
}