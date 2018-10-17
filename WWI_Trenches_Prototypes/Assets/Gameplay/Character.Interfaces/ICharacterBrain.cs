namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterBrain<TCharacter>
    {
        ICharacterState<TCharacter> State { get; }
    }
}
