using System;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyState : ICharacterState<GruntController>
    {
        public event EventHandler<IOrderArguments<GruntController>> StateChanged;
        public void ChangeStance(CharacterStance stance, IOrderArguments<GruntController> orderArguments)
        {
            var hasChanged = CurrentStance != stance;

            CurrentStance = stance;

            if (hasChanged) StateChanged?.Invoke(this, orderArguments);
        }

        public CharacterStance CurrentStance { get; private set; }
    }

    public class GruntBrain : MonoBehaviour, ICharacterBrain<GruntController>
    {
        [SerializeField]
        private EnemyOrder _idleOrder;
        [SerializeField]
        private EnemyOrder _attackOrder;

        private ICharacterOrder<GruntController> _currentOrder;

        public ICharacterState<GruntController> State { get; private set; }


        void OnEnable()
        {
            _idleOrder = new EnemyIdleOrder("Idle");
            _attackOrder = new EnemyShootOrder("Attack");
            State = new EnemyState();
            State.StateChanged += StateOnStateChanged;
        }

        void OnDestroy()
        {
            State.StateChanged -= StateOnStateChanged;
        }

        private void StateOnStateChanged(object sender, IOrderArguments<GruntController> arguments)
        {
            _currentOrder?.Deactivate(arguments);

            var newOrder = PickBehavior(arguments);

            _currentOrder = newOrder;

            _currentOrder?.Activate(arguments);

            _currentOrder?.Execute(arguments);
        }

        private EnemyOrder PickBehavior(IOrderArguments<GruntController> arguments)
        {
            if (arguments.CurrentTarget != null)
            {
                print("Enemy has attacking order");
                return _attackOrder;
            }

            return _idleOrder;
        }

    }
}