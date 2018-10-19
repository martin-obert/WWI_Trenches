using System;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyMemory : ICharacterMemory<GruntController>
    {
        public EnemyMemory()
        {
            _args = new CharacterMemoryEventArgs();
        }

        private readonly CharacterMemoryEventArgs _args;

        public event EventHandler<CharacterMemoryEventArgs> StateChanged;

        public void ChangeStance(CharacterStance stance)
        {
            var hasChanged = CurrentStance != stance;

            _args.LastStance = CurrentStance;
            _args.CurrentStance = stance;
            if (hasChanged) StateChanged ?.Invoke(this, _args);
        }

        public CharacterStance LastStance => _args.LastStance;

        public CharacterStance CurrentStance => _args.CurrentStance;
    }

    public class GruntBrain : MonoBehaviour, ICharacterBrain<GruntController>
    {
        private EnemyOrder _idleOrder;

        private EnemyOrder _attackOrder;

        private ISequence _currentSequence;

        private ISequence _startingSequence;

        private int _safeLoopBreakCounter = 100;

        public ICharacterMemory<GruntController> Memory { get; private set; }


        void OnEnable()
        {
            _idleOrder = new EnemyIdleOrder("Idle");
            _attackOrder = new EnemyShootOrder("Attack");
            Memory = new EnemyMemory();
        }

     
        //Todo: tohle se musí volat odjinud
        public void StateOnStateChanged(object sender, IOrderArguments<GruntController> arguments)
        {
            var newOrder = PickBehavior(arguments);

            newOrder?.Execute(arguments);
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

      
        public ISequence CurrentSequence
        {
            get { return _currentSequence; }
            set
            {
                if (_startingSequence == null)
                    _startingSequence = value;

                _currentSequence?.Chain(value);
                _currentSequence = value;
            }
        }


        public void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null)
        {
            _safeLoopBreakCounter = 100;
            while (_safeLoopBreakCounter >= 0)
            {
                _safeLoopBreakCounter--;

                var current = sequence ?? _startingSequence;

                if (sequence == _startingSequence || sequence == null) return;

                var orders = current.Orders;
                if (orders != null && orders.Length > 0)
                {
                    foreach (var order in orders)
                    {
                        order.Execute(arguments);
                    }
                }

                if (current.Next != null)
                {
                    sequence = current.Next;
                    continue;
                }

                break;
            }
        }
    }
}