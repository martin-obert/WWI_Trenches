using System;
using Assets.Gameplay.Instructions;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Inventory.Items;
using Assets.Gameplay.Mechanics;
using Assets.Gameplay.Zoning;
using Assets.IoC;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    public class BasicCharacter : MonoBehaviour, ICharacterProxy<BasicCharacter>
    {
        [SerializeField]
        private CharacterNavigator _navigator;

        private CharacterEquipment _equipment;

        [SerializeField] private CharacterBrain _brain;

        [SerializeField] private HumanoidSkeletonProxy _humanoidSkeleton;

        [SerializeField] private string _displayName;

        [SerializeField] private CharacterBehavior _behavior;

        [SerializeField] private Animator _animator;

        [SerializeField] private CharacterAttributesContainer _attributesContainer;

        [SerializeField] private ProxyZone _enemyScanner;

        private Cover _currentCover;

        public CharacterAttributesContainer Attributes => _attributesContainer;

        public int Id => GetInstanceID();

        public ICoverable CurrentCover => _currentCover;

        public string DisplayName => _displayName;

        public GameObject GameObject => gameObject;

        private MechanicsManager _mechanicsManager;

        #region Tohle je defakto jen implementace IOrderArguments?

        public ICharacterBrain<BasicCharacter> Brain => _brain;

        public ICharacterBehavior<BasicCharacter> Behavior => _behavior;

        public ICharacterNavigator Navigator => _navigator;

        public Animator Animator => _animator;

        public ProxyZone EnemyScanZone => _enemyScanner;

        public IOrderArguments<BasicCharacter> Components => new CharacterOrderArguments(this);

        public CharacterEquipment Equipment => _equipment;

        public ITargetable CurrentTarget { get; set; }

        public Vector3? Destination { get; set; }


        public float Visibility => Attributes.Visibility.Value();

        public float NoiseLevel => Attributes.NoiseLevel.Value();

        public IHumanoidSkeletonProxy SkeletonProxy => _humanoidSkeleton;

        #endregion

        void OnEnable()
        {
            _equipment = new CharacterEquipment();
            _equipment.BindEquipment(this);

            _enemyScanner.SubscribeTriggers(Inzone, Outzone);

            _brain.Memory.StateChanged += MemoryOnStateChanged;
        }

        void Start()
        {
            Injection.Instance.Get<MechanicsManager>(manager => _mechanicsManager = manager);
        }

      
        void OnDestroy()
        {
            _brain.Memory.StateChanged -= MemoryOnStateChanged;
            _enemyScanner.UnsubscribeTriggers(Inzone, Outzone);
        }

        //void LateUpdate()
        //{
        //    if (CurrentTarget != null)
        //    {
        //        //todo: pridat game objects s mecha mana scriptem
        //        //_mechanicsManager.MechanicsResolver.ResolveThreat(CurrentTarget, this);
        //    }
        //}

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagsHelper.CoverTag))
            {
                _currentCover = other.GetComponent<Cover>();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagsHelper.CoverTag) && _currentCover == other.GetComponent<Cover>())
            {
                _currentCover = null;
            }
        }

        private void Outzone(object sender, ProxyZone.ProxyZoneEvent e)
        {
            if (CurrentTarget == e.Targetable.Value)
            {
                CurrentTarget = null;
                e.Targetable.Value.CurrentNemesis = null;
            }
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent e)
        {
            if (e.Targetable.Value.CurrentNemesis == null && CurrentTarget == null)
            {
                CurrentTarget = e.Targetable.Value;
                e.Targetable.Value.CurrentNemesis = this;
            }
            
        }
        private void MemoryOnStateChanged(object sender, CharacterMemoryStateEventArgs characterMemoryStateEventArgs)
        {
            Behavior.RefreshStance(Brain, characterMemoryStateEventArgs.CurrentStance).Execute(Components);
        }

        public void Crouch()
        {
            Brain.Memory.ChangeStance(BasicStance.Crouching);
        }

        public void MoveTo(Vector3? point)
        {
            Navigator.Move(point);
            Brain.Memory.ChangeStance(BasicStance.Running);
        }

        public void Stop()
        {
            Navigator.Move(null);
        }

        public void Shoot()
        {
            if (_equipment != null && CurrentTarget != null && _equipment.MainWeapon != null)
            {
                var ranged = _equipment.MainWeapon.Item as RangedWeapon;
                if (ranged)
                {
                    ranged.RangedAttack(CurrentTarget);
                }
                else
                {
                    _equipment.MainWeapon.Item.MeleeAttack(CurrentTarget);
                }
            }
        }

        public void GotHitMelee(IWeapon weapon)
        {
            _mechanicsManager.MechanicsResolver.ResolveHit(weapon, this);
        }
        public void GotHitRanged(IProjectileLogic projectileLogic)
        {
            _mechanicsManager.MechanicsResolver.ResolveHit(projectileLogic, this);
        }

        public ITargetable CurrentNemesis { get; set; }

        public void Crawl()
        {
            Brain.Memory.ChangeStance(BasicStance.Crawling);
        }

        public void Run()
        {
            Brain.Memory.ChangeStance(BasicStance.Running);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerController))]
    public class CharacterEditor : Editor
    {
        void OnSceneGUI()
        {
            var zone = (target as BasicCharacter)?.EnemyScanZone;
            if (zone)
                ProxyZoneEditor.DrawZone(zone);
        }
    }

#endif
}