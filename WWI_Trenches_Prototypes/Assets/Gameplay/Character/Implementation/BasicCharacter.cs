﻿using System;
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
        [SerializeField] private CharacterBrain _brain;

        [SerializeField] private string _displayName;

        [SerializeField] private CharacterBehavior _behavior;

        [SerializeField] private CharacterNavigator _navigator;

        [SerializeField] private CharacterInventory _inventory;

        [SerializeField] private Animator _animator;

        [SerializeField] private CharacterAttributesContainer _attributesContainer;

        [SerializeField] private ProxyZone _enemyScanner;

        private Cover _currentCover;

        public CharacterAttributesContainer Attributes => _attributesContainer;

        public int Id => GetInstanceID();

        public ICoverable CurrentCover => _currentCover;

        public ThreatLevel ThreatLevel { get; set; }

        public string DisplayName => _displayName;

        public GameObject GameObject => gameObject;

        private MechanicsManager _mechanicsManager;

        #region Tohle je defakto jen implementace IOrderArguments?

        public ICharacterBrain<BasicCharacter> Brain => _brain;

        public ICombatBehavior<BasicCharacter> Behavior => _behavior;

        public ICharacterNavigator Navigator => _navigator;

        public Animator Animator => _animator;

        public ProxyZone EnemyScanZone => _enemyScanner;

        public IOrderArguments<BasicCharacter> OrderArguments => new CharacterOrderArguments(this);

        public CharacterInventory Inventory => _inventory;

        public ITargetable CurrentTarget { get; set; }

        public Vector3? Destination { get; set; }

       
        public float Visibility => Attributes.Visibility.Value();

        public float NoiseLevel => Attributes.NoiseLevel.Value();
        #endregion

        void Start()
        {
            _enemyScanner.SubscribeTriggers(Inzone, Outzone);

            _mechanicsManager =
                InjectService.Instance.GetInstance<MechanicsManager>(manager => _mechanicsManager = manager);

            _brain.Memory.StateChanged += MemoryOnStateChanged;

            if (_inventory)
            {
                _inventory.BindInventory(this);
            }
        }

      
        void OnDestroy()
        {
            _brain.Memory.StateChanged -= MemoryOnStateChanged;
            _enemyScanner.UnsubscribeTriggers(Inzone, Outzone);
        }

        void LateUpdate()
        {
            if (CurrentTarget != null)
            {
                _mechanicsManager.MechanicsResolver.ResolveThreat(CurrentTarget, this);
            }
        }

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
            CurrentTarget = e.Targetable.Value;
        }

        private void Inzone(object sender, ProxyZone.ProxyZoneEvent e)
        {
            if (CurrentTarget == e.Targetable.Value)
            {
                CurrentTarget = null;
            }
        }
        private void MemoryOnStateChanged(object sender, CharacterMemoryEventArgs characterMemoryEventArgs)
        {
            Behavior.RefreshStance(Brain, Brain.Memory).Execute(OrderArguments);
        }

        public void Aim()
        {
            Behavior.Aim(Brain).Execute(OrderArguments);
        }

        public void MoveTo(Vector3? point)
        {
            Navigator.Move(point);
        }

        public void Stop()
        {
            Navigator.Move(null);
        }

        public void Shoot()
        {
            if (_inventory && CurrentTarget != null && _inventory.MainWeapon != null)
            {
                var ranged = _inventory.MainWeapon as RangedWeapon;
                if (ranged)
                {
                    ranged.RangedAttack(CurrentTarget, this);
                }
                else
                {
                    _inventory.MainWeapon.MeleeAttack(CurrentTarget, this);
                }
            }
        }

        public void GotHitMelee(IWeapon weapon)
        {
            _mechanicsManager.MechanicsResolver.ResolveHit(weapon, this);
        }
        public void GotHitRanged(IProjectile projectile)
        {
            _mechanicsManager.MechanicsResolver.ResolveHit(projectile, this);
        }

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