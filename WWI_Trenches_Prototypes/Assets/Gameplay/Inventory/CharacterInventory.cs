using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField]
        private DedicatedInventorySlot<IWeapon> _mainWeapon;
        
        public IWeapon MainWeapon => _mainWeapon?.Item;

        public void EquipMainWeapon(IWeapon weapon)
        {
            _mainWeapon.Item = weapon;
        }
    }
}