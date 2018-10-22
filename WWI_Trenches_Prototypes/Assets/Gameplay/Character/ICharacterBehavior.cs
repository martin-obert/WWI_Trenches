using Assets.Gameplay.Instructions;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBehavior<TCharacter>
    {
        ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, ICharacterMemory<TCharacter> memory);
    }

    public interface ICombatBehavior<TCharacter> : ICharacterBehavior<TCharacter>
    {
        ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor);
        ISequenceExecutor Aim(ISequenceExecutor sequenceExecutor);
    }
}