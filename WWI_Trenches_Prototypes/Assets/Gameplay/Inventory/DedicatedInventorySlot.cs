using Assets.Gameplay.Inventory.Items;

namespace Assets.Gameplay.Inventory
{
    public class DedicatedInventorySlot<T> : ItemSlot where T : IItem
    {
        public T Item
        {
            get { return (T)StoredItem; }
            set { StoredItem = value; }
        }

    }
}