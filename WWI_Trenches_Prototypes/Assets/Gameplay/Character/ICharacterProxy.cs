using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    //Todo: tohle prejmenuj na character a udelej abstractni class s CharacterProxy
    public interface ICharacterProxy<TCharacter> : ITargetable, IProjectileTrigger
    {
        ProxyZone EnemyScanZone { get; }

        IOrderArguments<TCharacter> OrderArguments { get; }

        void Attack();

        void MoveTo(Vector3? point);

        void Stop();

        void Shoot();

        void Run();

        void Crawl();
    }
}