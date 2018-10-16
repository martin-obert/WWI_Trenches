using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public abstract class ItemSlot : ScriptableObject
    {
        protected IItem StoredItem { get; set; }

        public virtual bool IsOccupied => StoredItem != null && StoredItem.IsStackable && StoredItem.MaxStack < StoredItem.Amount;
    }
}