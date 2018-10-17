namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterOrder<TCharacter>
    {
        void Activate(IOrderArguments<TCharacter> arguments);
        void Deactivate(IOrderArguments<TCharacter> arguments);
        void Execute(IOrderArguments<TCharacter> arguments);
        string Name { get;  }
    }
}