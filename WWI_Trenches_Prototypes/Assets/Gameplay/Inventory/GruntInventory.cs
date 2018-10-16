using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Templates/Grunt", fileName = "GruntInventory")]
    public class GruntInventory :  InventoryTemplate
    {
        public Weapon WeaponPrefab;

        public override Weapon MainWeapon => WeaponPrefab;
    }
}