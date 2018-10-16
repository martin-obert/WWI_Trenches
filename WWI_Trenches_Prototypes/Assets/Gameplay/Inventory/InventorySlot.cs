using Assets.Gameplay.Inventory.Items;

namespace Assets.Gameplay.Inventory
{
    public class InventorySlot : ItemSlot
    {
        public IItem Item => StoredItem;
    }
}