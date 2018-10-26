using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay
{
    public interface IInstanciableProjectileLogic : IProjectileLogic
    {
        void ResetToStack();

        GameObject Prefab { get; }
    }

    public interface IProjectileLogic
    {
        float Damage { get; }
        float Speed { get; }
        float Lifetime { get; }
        void RayCastShot(IIdentificable shooter, Vector3 fromPosition, Vector3 directionNorm, float damage);
        void InstantShot(IIdentificable shooter, ITargetable target);
    }
}