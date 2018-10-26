using Assets.Gameplay.Instructions;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBehavior<TCharacter>
    {
        ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, BasicStance stance);
    }

}