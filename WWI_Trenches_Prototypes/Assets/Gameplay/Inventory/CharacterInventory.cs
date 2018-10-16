﻿using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField]
        private InventoryTemplate _template;

        void Start()
        {
            if (_template)
            {
                var instance = Instantiate(_template.MainWeapon);
                EquipMainWeapon(instance);
            }
        }

        [SerializeField]
        private DedicatedInventorySlot<IWeapon> _mainWeapon = new DedicatedInventorySlot<IWeapon>();
        
        public IWeapon MainWeapon => _mainWeapon?.Item;

        public void EquipMainWeapon(IWeapon weapon)
        {
            _mainWeapon.Item = weapon;
        }
    }
}