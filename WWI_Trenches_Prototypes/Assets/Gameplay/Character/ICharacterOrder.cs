namespace Assets.Gameplay.Character
{
    public interface ICharacterOrder<TCharacter> : IOrder
    {
        void Execute(IOrderArguments<TCharacter> arguments);
        
    }
}