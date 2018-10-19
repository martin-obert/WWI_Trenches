using Assets.Gameplay.Attributes;
using Assets.Gameplay.Instructions;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBehavior<TCharacter>
    {
        ISequenceExecutor PrepareToAttack(ISequenceExecutor sequenceExecutor);
        ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, ICharacterMemory<TCharacter> memory);
        ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor);
    }

    public interface INavigatorAttributes
    {
        ObservableAttribute<float> Speed { get; }
    }
}