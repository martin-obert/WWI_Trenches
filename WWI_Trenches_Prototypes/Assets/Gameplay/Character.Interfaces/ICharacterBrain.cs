using System;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface ICharacterBrain<in TCharacter>
    {
        void GiveOrder(TCharacter character);
    }

    public enum PlayerState
    {
        Idle,
        Running,
        Crawling,
        Covering,

        Shooting
    }
}
