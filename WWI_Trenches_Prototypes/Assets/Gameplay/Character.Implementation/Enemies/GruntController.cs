using System;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class GruntController : MonoBehaviour, ITargetable
    {
        [SerializeField]
        private ProxyZone _fireProxyZone;

        [SerializeField]
        private GruntBrain _gruntBrain;

        private BasicCharacterAttributesContainer _attributes;

        [SerializeField]
        private GruntNavigator _navigator;

        [SerializeField]
        private CharacterInventory _inventory;

        public CharacterInventory Inventory => _inventory;

        public GruntNavigator Navigator => _navigator;

        public string DisplayName => name;

        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        public void GotKilledBy(ITargetable killer)
        {
            EliminatedByOtherTarget?.Invoke(this, killer);
        }

        public ITargetable Target { get; private set; }

        void Start()
        {
            _fireProxyZone.SubscribeTriggers(Inzone, Outzone);

            _attributes = new BasicCharacterAttributesContainer();

            _navigator = GetComponent<GruntNavigator>();
        }

        void OnDestroy()
        {
            _fireProxyZone.UnsubscribeTriggers(Inzone, Outzone);
        }

        private void Outzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            var target = proxyZoneEvent.ZonedObject?.GetComponent<ITargetable>();
            if (target != null && target == Target && target.GameObject.CompareTag(TagsHelper.PlayerTag))
            {
                Target = null;
                _gruntBrain.State.ChangeStance(CharacterStance.Idle, GetArgs());
            }
        }

        private EnemyOrderArguments GetArgs()
        {
            return new EnemyOrderArguments(Target, Inventory, Navigator);
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            var target = proxyZoneEvent.ZonedObject?.GetComponent<ITargetable>();
            if (Target == null && target != null && Target != target && target.GameObject.CompareTag(TagsHelper.PlayerTag))
            {
                Target = target;
                print("Attacking player");
                _gruntBrain.State.ChangeStance(CharacterStance.Crouching, GetArgs());
            }
        }
    }
}
