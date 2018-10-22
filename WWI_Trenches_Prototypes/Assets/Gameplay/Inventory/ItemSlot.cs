using Assets.Gameplay.Inventory.Items;

namespace Assets.Gameplay.Inventory
{
    public abstract class ItemSlot
    {
        protected IItem StoredItem { get; set; }

        public virtual bool IsOccupied => StoredItem != null && StoredItem.IsStackable && StoredItem.MaxStack < StoredItem.Amount;
    }
}