using System;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Zoning;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class GruntBrain : MonoBehaviour, ICharacterBrain<GruntController>
    {
        [SerializeField] private OrderMapper<GruntController> _mapper;

        private ICharacterOrder<GruntController> _currentOrder;

        public void GiveOrder(GruntController character)
        {
            var arguments = new EnemyOrderArguments(character.Target, character.Inventory, character.Navigator);

            _currentOrder?.Deactivate(arguments);

            var newOrder = _mapper.GetBehaviorFromState(character);

            _currentOrder = newOrder;

            _currentOrder?.Activate(arguments);

            _currentOrder?.Execute(arguments);
        }
    }

    public class GruntOrderMapper : OrderMapper<GruntController>
    {
        private EnemyOrder _idleOrder;
        private EnemyOrder _attackOrder;

        public override ICharacterOrder<GruntController> GetBehaviorFromState(GruntController character)
        {
            if (character.Target != null)
            {
                return _attackOrder;
            }

            return _idleOrder;
        }
    }

    public class EnemyOrderArguments : IOrderArguments<GruntController>
    {
        public EnemyOrderArguments(ITargetable target, CharacterInventory inventory, ICharacterNavigator<GruntController> navigator)
        {
            Target = target;
            Inventory = inventory;
            Navigator = navigator;
        }

        public ICharacterNavigator<GruntController> Navigator { get; }

        public ITargetable Target { get; }

        public CharacterInventory Inventory { get; }
    }

    public class EnemyShootOrder : EnemyOrder
    {
        public EnemyShootOrder(string name) : base(name)
        {
        }

        protected override void Activate(EnemyOrderArguments arguments)
        {
            
        }

        protected override void Deactivate(EnemyOrderArguments arguments)
        {
            arguments.Inventory.MainWeapon?.StopFiring();
            
        }

        protected override void Execute(EnemyOrderArguments arguments)
        {
            arguments.Navigator.LookOn(arguments.Target.GameObject.transform);
            arguments.Inventory.MainWeapon?.StartFiring(arguments.Target.GameObject.transform.position);
        }
    }

    public class EnemyIdleOrder : EnemyOrder
    {
        public EnemyIdleOrder(string name) : base(name)
        {
        }

        protected override void Activate(EnemyOrderArguments arguments)
        {
            arguments.Inventory.MainWeapon?.StopFiring();
        }

        protected override void Deactivate(EnemyOrderArguments arguments)
        {
        }

        protected override void Execute(EnemyOrderArguments arguments)
        {
        }
    }

    public abstract class EnemyOrder : ICharacterOrder<GruntController>
    {
        protected EnemyOrder(string name)
        {
            Name = name;
        }

        public void Activate(IOrderArguments<GruntController> arguments)
        {
            Activate((EnemyOrderArguments)arguments);
        }

        public void Deactivate(IOrderArguments<GruntController> arguments)
        {
            Deactivate((EnemyOrderArguments)arguments);
        }

        public void Execute(IOrderArguments<GruntController> arguments)
        {
            Execute((EnemyOrderArguments)arguments);
        }

        protected abstract void Activate(EnemyOrderArguments arguments);
        protected abstract void Deactivate(EnemyOrderArguments arguments);
        protected abstract void Execute(EnemyOrderArguments arguments);

        public string Name { get; }
    }

    [RequireComponent(typeof(LookAtConstraint))]
    public class GruntNavigator : MonoBehaviour, ICharacterNavigator<GruntController>
    {
        private LookAtConstraint _lookAtConstraint;

        void Awake()
        {
            _lookAtConstraint = GetComponent<LookAtConstraint>();
            _lookAtConstraint.AddSource(new ConstraintSource());
        }

        public void LookOn(Transform target)
        {
            if (!target)
            {
                _lookAtConstraint.enabled = false;
            }
            else
            {
                _lookAtConstraint.enabled = true;
                _lookAtConstraint.SetSource(0, new ConstraintSource
                {
                    sourceTransform = target,
                    weight = 1
                });
            }

        }

        public void Teleport(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void Move(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }
    }

    public class GruntController : MonoBehaviour, ITargetable
    {
        [SerializeField]
        private ProxyZone _fireProxyZone;

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
            if (target != null && target == Target)
            {
                Target = null;
                _gruntBrain.GiveOrder(this);
            }
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent proxyZoneEvent)
        {
            var target = proxyZoneEvent.ZonedObject?.GetComponent<ITargetable>();
            if (Target == null && target != null && Target != target)
            {
                Target = target;
                _gruntBrain.GiveOrder(this);
            }
        }
    }
}
