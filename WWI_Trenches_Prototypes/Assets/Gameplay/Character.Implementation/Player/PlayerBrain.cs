using System;
using System.Linq;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public interface IOrderSet<TCharacter>
    {
        ICharacterOrder<TCharacter> GetOrder(Type type);
    }

    public class PlayerOrdersSet : ScriptableObject, ICharacterBehavior<PlayerController> //IOrderSet<PlayerController>
    {
        private PlayerOrder _runningOrder;
        private PlayerOrder _idleOrder;
        private PlayerOrder _crawlingOrder;
        private PlayerOrder _attackOrder;
        private PlayerOrder _coverOrder;
        private PlayerOrder _changeCourse;

        private PlayerOrder[] _container;

       void OnEnable()
        {
            //Todo: on state changed?


            _idleOrder = new PlayerIdleOrder("Idle");

            _runningOrder = new PlayerRunOrder("Run");

            _crawlingOrder = new PlayerCrawlOrder("Crawl");

            _attackOrder = new PlayerAimOrder("Attack");

            _coverOrder = new PlayerCoverOrder("Cover");

            _changeCourse = new PlayerChangeCourseOrder("ChangeCourse");

            _container = new[]
            {
                _attackOrder,
                _coverOrder,
                _crawlingOrder,
                _idleOrder,
                _runningOrder,
                _changeCourse
            };
        }


        public ICharacterOrder<PlayerController> GetOrder(Type type)
        {
            return _container.FirstOrDefault(x => x.GetType() == type);
        }

        public ISequenceExecutor PrepareToAttack(ISequenceExecutor sequenceExecutor)
        {
           return sequenceExecutor.Do(_attackOrder);
        }

        public ISequenceExecutor CourseChanged(ISequenceExecutor sequenceExecutor, ICharacterMemory<PlayerController> memory)
        {
            return sequenceExecutor.If(memory.CurrentStance == CharacterStance.Crawling, 
                new IOrder[] {_crawlingOrder},
                new IOrder[] {_runningOrder});
        }

        public ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor)
        {
            return sequenceExecutor.Do(_coverOrder);
        }
    }

    public interface ICharacterBehavior<TCharacter>
    {
        ISequenceExecutor PrepareToAttack(ISequenceExecutor sequenceExecutor);
        ISequenceExecutor CourseChanged(ISequenceExecutor sequenceExecutor, ICharacterMemory<TCharacter> memory);
        ISequenceExecutor HideSelf(ISequenceExecutor sequenceExecutor);
    }

    public class PlayerBrain : MonoBehaviour, ICharacterBrain<PlayerController>
    {
        

        private ISequence _currentSequence;
        private ISequence _startingSequence;
        private int _safeLoopBreakCounter = 100;
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

        public ICharacterMemory<PlayerController> Memory { get; private set; }


        [SerializeField]
        private PlayerOrdersSet _ordersSet;

        void OnEnable()
        {
            Memory = new PlayerCharacterMemory();

        }


        public void StateChanged(object sender, IOrderArguments<PlayerController> arguments)
        {
            
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
                        var playerOrder = order;

                        if (playerOrder != null)
                        {
                            playerOrder.Execute(arguments);
                        }
                        else
                        {
                            Debug.LogError("Cannot find type of " + order.Name);
                        }
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