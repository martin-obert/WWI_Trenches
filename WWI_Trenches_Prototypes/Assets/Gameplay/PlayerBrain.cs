using System;
using Assets.Gameplay.Units;
using UnityEngine;

namespace Assets.Gameplay
{
    public class PlayerBrain : MonoBehaviour, ICharacterBrain<Player>
    {
        [SerializeField]
        private PlayerBehavMapper _playerBehavMapper;

        public ICharacterBehavior<Player> CurrentBehavior { get; private set; }

        public void ChangeBehavior(Player character)
        {
            //Todo: tohle je mozna moc inicializace
            var args = new PlayerBehaviorArguments(character);

            CurrentBehavior?.Deactivate(args);

            CurrentBehavior = _playerBehavMapper.GetBehaviorFromState(character);
            print("Current behavior is " + CurrentBehavior.Name);

            CurrentBehavior?.Activate(args);

            //Todo: priprava na sekvenci, jakmile jich bude vice tak execute provede celou sekvenci neco jako ISequence : ICharacterBehavior a ISequence jich ma vice v sobe
            CurrentBehavior?.Execute(args);
        }
    }
}