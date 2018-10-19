namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterOrder<TCharacter> : IOrder
    {
        void Execute(IOrderArguments<TCharacter> arguments);
        
    }
}