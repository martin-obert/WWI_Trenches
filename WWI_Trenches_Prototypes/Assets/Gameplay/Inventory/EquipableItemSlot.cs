using System;
using Assets.Gameplay.Character;
using Assets.Gameplay.Inventory.Items;

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

        public void SetItem<TOwner>(T item,  TOwner owner) where TOwner: ICharacterProxy<TOwner>
        {
            StoredItem = item;
            if (item != null)
            {
                item.Equip(owner);
            }
        }
    }
}