﻿using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    [CreateAssetMenu(fileName = "Basic brain", menuName = "Character/Basic/Brain")]
    public class CharacterBrain : ScriptableObject, ICharacterBrain<BasicCharacter>
    {

        private ISequence _currentSequence;
        private ISequence _startingSequence;
        private int _safeLoopBreakCounter = 100;


        public ICharacterMemory<BasicCharacter> Memory { get; private set;  }

        void OnEnable()
        {
            Memory = new CharacterMemory();
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
                
                if (current == null) return;

                var orders = current.Orders;

                if (orders != null && orders.Length > 0)
                {
                    Debug.Log("Execution started " + orders.Length);

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

            CurrentSequence = null;
        }


    }
}