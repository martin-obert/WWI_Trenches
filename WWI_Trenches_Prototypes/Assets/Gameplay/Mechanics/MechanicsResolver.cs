﻿using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Mechanics
{
    public abstract class MechanicsResolver : ScriptableObject
    {
        public abstract void ResolveHit(IProjectile projectile, ITargetable victim);

        public abstract void ResolveThreat(ITargetable target, ITargetable observer);

        public abstract void ResolveHit(IWeapon projectile, ITargetable victim);
    }

    [CreateAssetMenu(fileName = "Basic mechanic resolver", menuName = "Mechanics/Basic mechanic resolver")]
    public class MechanicsResolverImpl : MechanicsResolver
    {

        [SerializeField, Range(0, 00.1f)] private float _distanceFactor = 0.001f;

        public override void ResolveHit(IProjectile projectile, ITargetable victim)
        {
            victim.Attributes.Health.Value(victim.Attributes.Health.Value() - 0.1f);
        }

        public override void ResolveThreat(ITargetable target, ITargetable observer)
        {
            var factor = Vector3.Distance(target.GameObject.transform.position, observer.GameObject.transform.position) * _distanceFactor;

            var noiseLevel = target.Attributes.NoiseLevel.Value() * factor;

            if (noiseLevel > 0.1)
            {
                if (noiseLevel > 0.4)
                {
                    if (noiseLevel > 0.6)
                    {
                        observer.Attributes.Threat.Value((int)ThreatLevel.High);
                    }
                    else
                    {
                        observer.Attributes.Threat.Value((int)ThreatLevel.Medium);
                    }
                }
                else
                {
                    observer.Attributes.Threat.Value((int)ThreatLevel.Low);
                }
            }
            else
            {
                observer.Attributes.Threat.Value((int)ThreatLevel.None);
            }
        }

        public override void ResolveHit(IWeapon projectile, ITargetable victim)
        {
            victim.Attributes.Health.Value(victim.Attributes.Health.Value() - 0.1f);
        }
    }
}