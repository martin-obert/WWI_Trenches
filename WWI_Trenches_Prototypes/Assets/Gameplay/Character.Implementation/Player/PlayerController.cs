using System;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Instructions;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Units;
using Assets.Gameplay.Units.Enemy;
using Assets.Gameplay.Zoning;
using Assets.TileGenerator;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public static class PlayerHelpers
    {
        public static Vector3 GetEndPoint(PlayerController player, TiledTerrain terrain)
        {
            return new Vector3(player.transform.position.x, terrain.EndPoint.position.y, terrain.EndPoint.position.z);
        }

        public static Vector3 GetMinWeaponDistanceToEnemy(Transform player, Transform enemy, float playerWeaponRange)
        {
            return (enemy.position - player.position).normalized * playerWeaponRange;
        }
    }

    public sealed class TagsHelper
    {
        public static readonly string EnemyTag = "Enemy";
        public static readonly string PlayerTag = "Player";
        public static readonly string CoverTag = "Cover";
    }

    public enum ThreatLevel
    {
        None,
        EnemyIsNear,
    }

    public interface ICharacterController
    {

    }

    [Obsolete("User character instead"), RequireComponent(typeof(PlayerBrain), typeof(NavMeshAgent))]
    public class PlayerController : Singleton<PlayerController>,  ICharacterProxy<PlayerController>
    {
        [SerializeField] private ProxyZone _enemyScanZone;

        private ICharacterBrain<PlayerController> _playerBrain;

        private Animator _animator;

        public CharacterAttributesContainer Attributes { get; private set; }

        public PlayerCharacterNavigator Navigator { get; private set; }

        private CharacterInventory _characterInventory;

        //Todo: tohle se presune nekam
        public Vector3? Destination { get; set; }

        public string DisplayName => name;

        public int Id => gameObject.GetInstanceID();

        public GameObject GameObject => gameObject;

        public ITargetable CurrentEnemy { get; private set; }

        //Todo: Invoke from death method
        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        private Cover _selectedCover;

        private ICharacterBehavior<PlayerController> _behavior;

        public void GotKilledBy(ITargetable killer)
        {
            EliminatedByOtherTarget?.Invoke(this, killer);
        }

        public bool IsVisibleTo(ITargetable targetable)
        {
            //print(_playerBrain.State.CurrentStance);
            return _playerBrain.Memory.CurrentStance != CharacterStance.Crawling;
        }

        public event EventHandler VisibilityChanged;

        void OnEnable()
        {
            Attributes = new CharacterAttributesContainer();

            _playerBrain = GetComponent<PlayerBrain>();

            _characterInventory = GetComponent<CharacterInventory>();

            _animator = GetComponentInChildren<Animator>();

            Navigator = new PlayerCharacterNavigator();

            if (_enemyScanZone)
            {
                _enemyScanZone.SubscribeTriggers(InZoneEventHandler, OutZoneEventHandler);
            }
        }

        private void OutZoneEventHandler(object sender, ProxyZone.ProxyZoneEvent eventArgs)
        {
            if (eventArgs.ZonedObject && eventArgs.ZonedObject.CompareTag(TagsHelper.EnemyTag))
            {
                var enemy = eventArgs.ZonedObject.GetComponent<ITargetable>();
                if (CurrentEnemy == enemy)
                {
                    CurrentEnemy = null;
                }
            }
        }

        private void InZoneEventHandler(object sender, ProxyZone.ProxyZoneEvent eventArgs)
        {
            print("is " + eventArgs.ZonedObject);
            if (eventArgs.ZonedObject && eventArgs.ZonedObject.CompareTag(TagsHelper.EnemyTag))
            {
                var enemy = eventArgs.ZonedObject.GetComponent<ITargetable>();
                if (enemy == null)
                {
                    Debug.LogError("This is marked as enemy but has no enemy component " + eventArgs.ZonedObject);
                }
                else
                {

                    CurrentEnemy = enemy;
                }
            }
        }
        void Start()
        {
            CreateSingleton(this);
        }

        private void StateOnStateChanged(object sender, IOrderArguments<PlayerController> e)
        {
            VisibilityChanged?.Invoke(this, null);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if(!_selectedCover)
                Destination = PlayerHelpers.GetEndPoint(this, TerrainManager.Instance.CurrentTerrain);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!_selectedCover)
                    Destination = PlayerHelpers.GetEndPoint(this, TerrainManager.Instance.CurrentTerrain);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
            }
        }


        //Todo: moc inicializace
        private PlayerOrderArguments GetOrderArguments()
        {
            return new PlayerOrderArguments(_animator, Destination, CurrentEnemy, Navigator, Attributes, _characterInventory);
        }

     
        void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag(TagsHelper.CoverTag))
            {
                var cover = other.GetComponent<Cover>();
                if (cover == _selectedCover)
                {
                    _selectedCover = null;
                    //Todo: spis nabidnout cover

                    HideInCover();
                }


            }
        }

        void OnTriggerExit(Collider other)
        {

        }

        public void Spawn(Vector3 startPointPosition)
        {
            Navigator.Teleport(startPointPosition);
        }

        public void OnProjectileTriggered(IProjectile projectile)
        {
        }

        public IOrderArguments<PlayerController> OrderArguments { get; }

        public void Attack()
        {
            _behavior.PrepareToAttack(_playerBrain).Execute(GetOrderArguments());
        }

        public void ChangeCourse()
        {
            _behavior.CourseChanged(_playerBrain, _playerBrain.Memory).Execute(GetOrderArguments());
        }

        public void Stop()
        {
            Destination = null;
            _behavior.CourseChanged(_playerBrain, _playerBrain.Memory).Execute(GetOrderArguments());
        }

        public void Shoot()
        {
            throw new NotImplementedException();
        }

        public void HideInCover()
        {
            _behavior.HideSelf(_playerBrain).Execute(GetOrderArguments());
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        void OnSceneGUI()
        {
            var zone = serializedObject.FindProperty("_enemyScanZone").objectReferenceValue as ProxyZone;
            if (zone)
                ProxyZoneEditor.DrawZone(zone);
        }
    }

#endif
}