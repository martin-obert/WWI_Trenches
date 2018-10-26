using System;
using Assets.Gameplay.Character;
using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public class EquipableItemSlot<T> : ItemSlot where T : IItem, IEquipable
    {
        public class InventorySlotEventArgs : EventArgs
        {
            public T PreviousItem { get; set; }
            public T CurrentItem { get; set; }
        }

        public event EventHandler<InventorySlotEventArgs> ItemChanged;

        public T Item => (T)StoredItem;

        public void SetItem<TOwner>(T item, TOwner owner) where TOwner : ICharacterProxy<TOwner>
        {
            var hasChanged = (StoredItem == null && item != null) || (item == null && StoredItem != null) || item != null && StoredItem != null && item.Id != StoredItem.Id;

            var temp = hasChanged ? StoredItem : null;

            StoredItem = item;

            if (Item != null && hasChanged)
            {
                Item.Equip(owner);
            }

            if (hasChanged)
            {
                Debug.Log("Item changed");
                ItemChanged?.Invoke(this, new InventorySlotEventArgs
                {
                    CurrentItem = Item,
                    PreviousItem = (T)temp
                });
            }


        }
    }
}