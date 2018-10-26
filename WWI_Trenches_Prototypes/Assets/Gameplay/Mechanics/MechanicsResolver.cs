using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Mechanics
{
    public abstract class MechanicsResolver : ScriptableObject
    {
        public abstract void ResolveHit(IProjectileLogic projectileLogic, ITargetable victim);

        public abstract void ResolveThreat(ITargetable target, ITargetable observer);

        public abstract void ResolveHit(IWeapon projectile, ITargetable victim);
    }
}