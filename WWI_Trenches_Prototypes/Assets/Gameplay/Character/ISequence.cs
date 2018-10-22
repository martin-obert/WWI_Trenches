namespace Assets.Gameplay.Character
{
    public interface ISequence
    {
        string Name { get; }

        void Execute<T>(IOrderArguments<T> arguments);
    }
}