using System;
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


        public ISequenceExecutor RefreshStance(ISequenceExecutor sequenceExecutor, BasicStance stance)
        {
            switch (stance)
            {
                case BasicStance.Idle:
                case BasicStance.Running:
                    return sequenceExecutor.Do(_moveOrder);
                case BasicStance.Crawling:
                    return sequenceExecutor.Do(_crawlingOrder);
                case BasicStance.Sitting:
                    return sequenceExecutor.Do(_coverOrder);
                case BasicStance.Crouching:
                    //Todo: jen crouch
                    return sequenceExecutor.Do(_aimOrder);
                default:
                    throw new NotImplementedException();
            }
        }

        public ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_coverOrder);
        }
    }
}