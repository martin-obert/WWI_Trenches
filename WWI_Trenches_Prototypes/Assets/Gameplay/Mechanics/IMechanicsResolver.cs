using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character;
using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Mechanics
{
    public abstract class MechanicsResolver : ScriptableObject
    {
        public abstract void ResolveHit(IProjectile projectile, ITargetable victim);

        public abstract void ResolveThreat(ITargetable target, ITargetable observer);

        public abstract void ResolveHit(IWeapon projectile, BasicCharacter victim);
    }

    public class MechanicsManager : Singleton<MechanicsManager>
    {
        [SerializeField]
        private MechanicsResolver _mechanicsResolver;

        public MechanicsResolver MechanicsResolver => _mechanicsResolver;

        void OnEnable()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }


    }
}
