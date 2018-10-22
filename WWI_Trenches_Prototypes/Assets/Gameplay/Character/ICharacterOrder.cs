namespace Assets.Gameplay.Character
{
    public interface ICharacterOrder<TCharacter> : ISequence
    {
        void Execute(IOrderArguments<TCharacter> arguments);
        
    }
}