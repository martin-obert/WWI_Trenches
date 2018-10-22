using Assets.Gameplay.Character.Implementation.Orders;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    [CreateAssetMenu(fileName = "Basic behavior", menuName = "Character/Basic/Behavior")]
    public class CharacterBehavior : ScriptableObject, ICombatBehavior<BasicCharacter>
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

        public ISequenceExecutor Aim(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_aimOrder);
        }

        public ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, ICharacterMemory<BasicCharacter> memory)
        {

            return sequenceExecutor.Decide(() => memory.CurrentStance == BasicStance.Crawling, new[] { _crawlingOrder }, new[] { _moveOrder });
        }

        public ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_coverOrder);
        }
    }
}