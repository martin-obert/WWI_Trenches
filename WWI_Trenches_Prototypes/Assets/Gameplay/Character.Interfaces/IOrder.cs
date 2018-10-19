namespace Assets.Gameplay.Character.Interfaces
{
    public interface IOrder
    {
        string Name { get; }

        void Execute<T>(IOrderArguments<T> arguments);
    }
}