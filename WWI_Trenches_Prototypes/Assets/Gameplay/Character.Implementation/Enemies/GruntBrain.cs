using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class GruntBrain : MonoBehaviour, ICharacterBrain<GruntController>
    {
        [SerializeField]
        private EnemyOrder _idleOrder;
        [SerializeField]
        private EnemyOrder _attackOrder;


        private ICharacterOrder<GruntController> _currentOrder;

        public void GiveOrder(GruntController character)
        {
            var arguments = new EnemyOrderArguments(character.Target, character.Inventory, character.Navigator);

            _currentOrder?.Deactivate(arguments);

            var newOrder = PickBehavior(character);

            _currentOrder = newOrder;

            _currentOrder?.Activate(arguments);

            _currentOrder?.Execute(arguments);
        }

        private EnemyOrder PickBehavior(GruntController character)
        {
            if (character.Target != null)
            {
                return _attackOrder;
            }

            return _idleOrder;
        }
    }
}