using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerBrain : MonoBehaviour, ICharacterBrain<PlayerController>
    {
        private PlayerOrder _runningOrder;
        private PlayerOrder _idleOrder;
        private PlayerOrder _crawlingOrder;
        private PlayerOrder _attackOrder;
        private PlayerOrder _coverOrder;
        private PlayerOrder _changeCourse;
        private PlayerOrder[] _container;
        public ICharacterState<PlayerController> State { get; private set; }

        void OnEnable()
        {
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

            State = new PlayerCharacterState();
        }

        

        public void ProcessSequence(PlayerOrderArguments args, params Type[] orders)
        {
            foreach (var order in orders)
            {
                var playerOrder = _container.FirstOrDefault(x => x.GetType() == order);

                if (playerOrder != null)
                {
                    playerOrder.Execute(args);
                }
                else
                {
                    Debug.LogError("Cannot find type of " + order.Name);
                }
            }
        }

        public ICharacterOrder<PlayerController> PickStanceAccordingToStance()
        {
            switch (State.CurrentStance)
            {
                case CharacterStance.Idle:
                    return _idleOrder;
                case CharacterStance.Running:
                    return _runningOrder;
                case CharacterStance.Crawling:
                    return _crawlingOrder;
                case CharacterStance.Aiming:
                    return _attackOrder;
                case CharacterStance.Sitting:
                    return _coverOrder;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}