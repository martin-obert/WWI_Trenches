using System;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Zoning;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    [RequireComponent(typeof(CharacterInventory))]
    public class BasicCharacter : MonoBehaviour, ICharacterProxy<BasicCharacter>
    {
        [SerializeField]
        private CharacterBrain _brain;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private CharacterBehavior _behavior;

        [SerializeField]
        private CharacterNavigator _navigator;

        [SerializeField]
        private CharacterInventory _inventory;

        [SerializeField]
        private Animator _animator;

        [SerializeField] private CharacterAttributesContainer _attributesContainer;

        [SerializeField] private ProxyZone _enemyScanner;

        public CharacterAttributesContainer Attributes => _attributesContainer;

        public int Id => GetInstanceID();

        public string DisplayName => _displayName;

        public GameObject GameObject => gameObject;

        #region Tohle je defakto jen implementace IOrderArguments?
        public ICharacterBrain<BasicCharacter> Brain => _brain;

        public ICharacterBehavior<BasicCharacter> Behavior => _behavior;

        public ICharacterNavigator<BasicCharacter> Navigator => _navigator;

        public Animator Animator => _animator;

        public ProxyZone EnemyScanZone => _enemyScanner;

        public IOrderArguments<BasicCharacter> OrderArguments => new CharacterOrderArguments(this);

        public CharacterInventory Inventory => _inventory;

        public ITargetable CurrentTarget { get; set; }
        public Vector3? Destination { get; set; }

        #endregion

        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        public event EventHandler VisibilityChanged;

        void Start()
        {
            _enemyScanner.SubscribeTriggers(Inzone, Outzone);
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


        public void Attack()
        {
            Behavior.PrepareToAttack(Brain).Execute(OrderArguments);
        }

        public void MoveTo(Vector3? point)
        {
            Navigator.Move(point);
            Behavior.RefreshStance(Brain, Brain.Memory).Execute(OrderArguments);
        }


        public void Stop()
        {
            Navigator.Move(null);
        }

        public void Shoot()
        {
            throw new System.NotImplementedException();
        }

        public void GotKilledBy(ITargetable killer)
        {
            throw new NotImplementedException();
        }

        public bool IsVisibleTo(ITargetable targetable)
        {
            throw new NotImplementedException();
        }

        public void OnProjectileTriggered(IProjectile projectile)
        {
            throw new NotImplementedException();
        }


        public void Crawl()
        {
            Brain.Memory.ChangeStance(CharacterStance.Crawling);
            Behavior.RefreshStance(Brain, Brain.Memory).Execute(OrderArguments);
        }

        public void Run()
        {
            Brain.Memory.ChangeStance(CharacterStance.Running);
            Behavior.RefreshStance(Brain, Brain.Memory).Execute(OrderArguments);
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
