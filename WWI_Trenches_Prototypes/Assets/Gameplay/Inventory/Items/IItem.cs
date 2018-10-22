using Assets.Gameplay.Character;

namespace Assets.Gameplay.Inventory.Items
{
    public interface IItem : IIdentificable
    {
        bool IsStackable { get; }
        string Name { get; }
        int MaxStack { get; }
        int Amount { get; }
    }
}