using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay.Projectiles
{
    public class ProjectileLogic : IProjectileLogic
    {
        public float Speed { get; }

        public float Lifetime { get; }

        public float Damage { get; private set; }

        public void RayCastShot(IIdentificable shooter, Vector3 fromPosition, Vector3 directionNorm, float dataDamage)
        {
            Damage = dataDamage;

            var ray = new Ray(fromPosition, directionNorm);

            //Todo: add range limit

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hit.collider.GetComponent<ITargetable>()?.GotHitRanged(this);
            }
        }

        public void InstantShot(IIdentificable shooter, ITargetable target)
        {
            target?.GotHitRanged(this);
        }
    }
}
