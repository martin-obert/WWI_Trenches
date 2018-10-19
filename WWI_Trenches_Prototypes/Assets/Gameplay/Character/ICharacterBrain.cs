using Assets.Gameplay.Instructions;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBrain<TCharacter> : ISequenceExecutor
    {
        ICharacterMemory<TCharacter> Memory { get; }
    }
}
