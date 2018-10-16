using System;
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
         void OnEnable()
        {
            //Todo: prenest do editoru
            _idleOrder = new PlayerIdleOrder("Idle");
            _runningOrder = new PlayerRunOrder("Run");
            _crawlingOrder = new PlayerCrawlOrder("Crawl");
            _attackOrder = new PlayerShootOrder("Attack");
            _coverOrder = new PlayerCoverOrder("Cover");
        }


        private ICharacterOrder<PlayerController> _currentOrder;

        public void GiveOrder(PlayerController character)
        {
            //Todo: tohle je mozna moc inicializace
            var args = new PlayerOrderArguments(character);

            _currentOrder?.Deactivate(args);

            _currentOrder = PickBehavior(character);

            print("Order: "+ _currentOrder.Name);
            _currentOrder?.Activate(args);

            //Todo: priprava na sekvenci, jakmile jich bude vice tak execute provede celou sekvenci neco jako ISequence : ICharacterBehavior a ISequence jich ma vice v sobe
            _currentOrder?.Execute(args);

        }

        private ICharacterOrder<PlayerController> PickBehavior(PlayerController character)
        {
            switch (character.State)
            {
                case PlayerState.Idle:
                    return _idleOrder;
                case PlayerState.Running:
                    return _runningOrder;
                case PlayerState.Crawling:
                    return _crawlingOrder;
                case PlayerState.Shooting:
                    return _attackOrder;
                case PlayerState.Covering:
                    return _coverOrder;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}