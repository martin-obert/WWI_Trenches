namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterOrder<TCharacter>
    {
        void Execute(IOrderArguments<TCharacter> arguments);
        string Name { get;  }
    }
}