using Assets.Gameplay.Character.Implementation.Orders;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    [CreateAssetMenu(fileName = "Basic behavior", menuName = "Character/Basic/Behavior")]
    public class CharacterBehavior : ScriptableObject, ICharacterBehavior<BasicCharacter>
    {
        [SerializeField]
        private Order _moveOrder;
        [SerializeField]
        private Order _idleOrder;
        [SerializeField]
        private Order _crawlingOrder;
        [SerializeField]
        private Order _aimOrder;
        [SerializeField]
        private Order _coverOrder;

        public ISequenceExecutor PrepareToAttack(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_aimOrder);
        }

        public ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, ICharacterMemory<BasicCharacter> memory)
        {
            Debug.Log("Memory " + memory);

            return sequenceExecutor.Decide(() => memory.CurrentStance == CharacterStance.Crawling, new[] { _crawlingOrder }, new[] { _moveOrder });
        }

        public ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_coverOrder);
        }
    }
}