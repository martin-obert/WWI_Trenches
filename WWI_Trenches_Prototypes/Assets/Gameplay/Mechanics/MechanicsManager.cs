using Assets.Gameplay.Abstract;
using UnityEngine;

namespace Assets.Gameplay.Mechanics
{
    public class MechanicsManager : Singleton<MechanicsManager>
    {
        [SerializeField]
        private MechanicsResolver _mechanicsResolver;

        public MechanicsResolver MechanicsResolver => _mechanicsResolver;

        protected override void OnAwakeHandle()
        {
            CreateSingleton(this);
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }
    }
}
