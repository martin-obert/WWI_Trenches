using System;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerBrain : MonoBehaviour, ICharacterBrain<PlayerController>
    {
        [SerializeField]
        private PlayerOrderMapper _playerOrderMapper;


        private ICharacterOrder<PlayerController> _currentOrder;

        public void GiveOrder(PlayerController character)
        {
            //Todo: tohle je mozna moc inicializace
            var args = new PlayerOrderArguments(character);

            _currentOrder?.Deactivate(args);

            _currentOrder = PickBehavior(character);

            _currentOrder?.Activate(args);

            //Todo: priprava na sekvenci, jakmile jich bude vice tak execute provede celou sekvenci neco jako ISequence : ICharacterBehavior a ISequence jich ma vice v sobe
            _currentOrder?.Execute(args);

        }

        private ICharacterOrder<PlayerController> PickBehavior(PlayerController character)
        {
            return _playerOrderMapper.GetBehaviorFromState(character);
        }
    }
}