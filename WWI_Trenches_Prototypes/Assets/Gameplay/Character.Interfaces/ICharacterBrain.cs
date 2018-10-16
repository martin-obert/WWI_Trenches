using UnityEngine;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterBrain<in TCharacter>
    {
        void ChangeBehavior(TCharacter character);
    }

    public enum PlayerState
    {
        Idle,
        Running,
        Crawling,
        Covering,

        Attacking
    }
}
