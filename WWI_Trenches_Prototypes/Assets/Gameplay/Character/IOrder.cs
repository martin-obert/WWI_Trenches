namespace Assets.Gameplay.Character
{
    public interface IOrder
    {
        string Name { get; }

        void Execute<T>(IOrderArguments<T> arguments);
    }
}