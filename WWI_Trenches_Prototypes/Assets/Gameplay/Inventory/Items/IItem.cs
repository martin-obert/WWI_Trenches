namespace Assets.Gameplay.Inventory.Items
{
    public interface IItem
    {
        bool IsStackable { get; }
        string Name { get; }
        int MaxStack { get; }
        int Amount { get; }
    }
}