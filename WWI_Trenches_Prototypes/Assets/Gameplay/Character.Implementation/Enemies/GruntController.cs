using System;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Zoning;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class GruntController : MonoBehaviour, ITargetable, IProjectileTrigger
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

        public bool IsVisibleTo(ITargetable targetable)
        {
            return true;
        }

        public event EventHandler VisibilityChanged;

        public ITargetable Target { get; private set; }

        void Start()
        {
            _fireProxyZone.SubscribeTriggers(Inzone, Outzone);

            _attributes = new BasicCharacterAttributesContainer();

            _navigator = GetComponent<GruntNavigator>();

            _gruntBrain.State.StateChanged += StateOnStateChanged;
        }

        private void StateOnStateChanged(object sender, IOrderArguments<GruntController> e)
        {
            _gruntBrain.StateOnStateChanged(null, GetArgs());
        }

        void OnDestroy()
        {
            _gruntBrain.State.StateChanged -= StateOnStateChanged;
            _fireProxyZone.UnsubscribeTriggers(Inzone, Outzone);
        }
        private void Inzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            var target = proxyZoneEvent.ZonedObject?.GetComponent<ITargetable>();
            if (Target == null && target != null && Target != target && target.GameObject.CompareTag(TagsHelper.PlayerTag))
            {
                Target = target;
                Target.VisibilityChanged += TargetOnVisibilityChanged;
                TryShootTarget();
            }
        }

        private void TargetOnVisibilityChanged(object sender, EventArgs e)
        {
            TryShootTarget();
        }


        private void TryShootTarget()
        {
            //Todo: Tohle by mohl delat brain
            if (Target != null && Target.IsVisibleTo(this) && _gruntBrain.State.CurrentStance != CharacterStance.Aiming)
            {
                print("Attacking player");

                _gruntBrain.State.ChangeStance(CharacterStance.Aiming, GetArgs());

                InvokeRepeating(nameof(InvokeFire), 1, _inventory.MainWeapon.AttackSpeed);
            }
        }

        private void Outzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            var target = proxyZoneEvent.ZonedObject?.GetComponent<ITargetable>();
            if (target != null && target == Target && target.GameObject.CompareTag(TagsHelper.PlayerTag))
            {
                Target.VisibilityChanged -= TargetOnVisibilityChanged;
                Target = null;
                _gruntBrain.State.ChangeStance(CharacterStance.Idle, GetArgs());
                CancelInvoke(nameof(InvokeFire));
            }

        }

        private EnemyOrderArguments GetArgs()
        {
            return new EnemyOrderArguments(Target, Inventory, Navigator);
        }


        void Update()
        {

        }

        private void InvokeFire()
        {
            _inventory.MainWeapon.FireOnce(Target.GameObject.transform.position, gameObject.GetInstanceID());
        }

        public void OnProjectileTriggered(IProjectile projectile)
        {
            Destroy(gameObject);
            Application.Quit();
        }

        public int Id => gameObject.GetInstanceID();
    }
}
