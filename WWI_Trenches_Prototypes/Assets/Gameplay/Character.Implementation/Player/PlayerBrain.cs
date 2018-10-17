using System;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerBrain : MonoBehaviour, ICharacterBrain<PlayerController>
    {
        private ICharacterOrder<PlayerController> _currentOrder;

        private PlayerOrder _runningOrder;
        private PlayerOrder _idleOrder;
        private PlayerOrder _crawlingOrder;
        private PlayerOrder _attackOrder;
        private PlayerOrder _coverOrder;

        public ICharacterState<PlayerController> State { get; private set; }

        void OnEnable()
        {
            //Todo: prenest do editoru
            _idleOrder = new PlayerIdleOrder("Idle");
            _runningOrder = new PlayerRunOrder("Run");
            _crawlingOrder = new PlayerCrawlOrder("Crawl");
            _attackOrder = new PlayerShootOrder("Attack");
            _coverOrder = new PlayerCoverOrder("Cover");
            State = new PlayerCharacterState();
            State.StateChanged += StateOnStateChanged;
        }

        void OnDestroy()
        {
            State.StateChanged -= StateOnStateChanged;
        }

        private void StateOnStateChanged(object sender, IOrderArguments<PlayerController> args)
        {
            _currentOrder?.Deactivate(args);

            _currentOrder = PickStance();

            print("Order: " + _currentOrder.Name);
            _currentOrder?.Activate(args);

            //Todo: priprava na sekvenci, jakmile jich bude vice tak execute provede celou sekvenci neco jako ISequence : ICharacterBehavior a ISequence jich ma vice v sobe
            _currentOrder?.Execute(args);
        }
      
        private ICharacterOrder<PlayerController> PickStance()
        {
            switch (State.CurrentStance)
            {
                case CharacterStance.Idle:
                    return _idleOrder;
                case CharacterStance.Running:
                    return _runningOrder;
                case CharacterStance.Crawling:
                    return _crawlingOrder;
                case CharacterStance.Crouching:
                    return _attackOrder;
                case CharacterStance.Sitting:
                    return _coverOrder;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}