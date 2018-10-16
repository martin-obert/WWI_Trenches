namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterAttribute
    {
        object MinValue { get; }
        object CurrentValue { get; set; }
        object MaxValue { get; }
        string DisplayName { get; }
        int Hash { get; }
    }
}