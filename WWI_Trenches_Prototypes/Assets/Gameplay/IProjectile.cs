using UnityEngine;

namespace Assets.Gameplay
{
    public interface IProjectile
    {
        int ShotFromWeaponId { get; }
        int ShotByCharacterId { get; }
        float Speed { get; }
        float Lifetime { get; }
        Vector3? StartingPosition { get; }
        Vector3? DirectionNorm { get; }
        bool IsFired { get; }
        void Shoot(Vector3 fromPosition, Vector3 directionNorm);
        void ResetToStack();
    }
}