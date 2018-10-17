using System;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Implementation.Player.Orders;
using Assets.Gameplay.Character.Interfaces;
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

    [RequireComponent(typeof(PlayerBrain), typeof(NavMeshAgent))]
    public class PlayerController : Singleton<PlayerController>, ITargetable, IProjectileTrigger
    {
        [SerializeField] private ProxyZone _enemyScanZone;

        private PlayerBrain _playerBrain;

        private Animator _animator;

        public BasicCharacterAttributesContainer Attributes { get; private set; }

        public PlayerCharacterNavigator Navigator { get; private set; }

        private CharacterInventory _characterInventory;

        //Todo: tohle se presune nekam
        public Vector3 Destination { get; set; }

        public string DisplayName => name;

        public int Id => gameObject.GetInstanceID();

        public GameObject GameObject => gameObject;

        public ITargetable CurrentEnemy { get; private set; }

        //Todo: Invoke from death method
        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        private Cover _selectedCover;

        public void GotKilledBy(ITargetable killer)
        {
            EliminatedByOtherTarget?.Invoke(this, killer);
        }

        public bool IsVisibleTo(ITargetable targetable)
        {
            //print(_playerBrain.State.CurrentStance);
            return _playerBrain.State.CurrentStance != CharacterStance.Crawling;
        }

        public event EventHandler VisibilityChanged;

        void OnEnable()
        {
            Attributes = new BasicCharacterAttributesContainer();

            _playerBrain = GetComponent<PlayerBrain>();

            _characterInventory = GetComponent<CharacterInventory>();

            _animator = GetComponentInChildren<Animator>();

            Navigator = new PlayerCharacterNavigator(GetComponent<NavMeshAgent>(), Attributes, transform);

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
            _playerBrain.State.StateChanged+= StateOnStateChanged;
            CreateSingleton(this);
        }

        private void StateOnStateChanged(object sender, IOrderArguments<PlayerController> e)
        {
            VisibilityChanged?.Invoke(this, null);
        }

        void OnDestroy()
        {
            _playerBrain.State.StateChanged -= StateOnStateChanged;
            GCSingleton(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if(!_selectedCover)
                Destination = PlayerHelpers.GetEndPoint(this, TerrainManager.Instance.CurrentTerrain);
                Crawl();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!_selectedCover)
                    Destination = PlayerHelpers.GetEndPoint(this, TerrainManager.Instance.CurrentTerrain);
                Run();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackEnemy();
            }
        }


        //Todo: moc inicializace
        private PlayerOrderArguments GetOrderArguments()
        {
            return new PlayerOrderArguments(_animator, Destination, CurrentEnemy, Navigator, Attributes, _characterInventory);
        }

        public void Crawl()
        {
            _playerBrain.State.ChangeStance(CharacterStance.Crawling, GetOrderArguments());
            _playerBrain.ProcessSequence(GetOrderArguments(), typeof(PlayerCrawlOrder));
        }

        public void Run()
        {
            _playerBrain.State.ChangeStance(CharacterStance.Running, GetOrderArguments());
            _playerBrain.ProcessSequence(GetOrderArguments(), typeof(PlayerChangeCourseOrder), typeof(PlayerRunOrder));
        }

        public void ChangeCourse()
        {
            
        }

        public void Stop()
        {
        }

        public void TakeCover(Cover cover)
        {
            _selectedCover = cover;

            Destination = cover.transform.position;

            _playerBrain.State.ChangeStance(CharacterStance.Crawling, GetOrderArguments());
            _playerBrain.ProcessSequence(GetOrderArguments(), typeof(PlayerChangeCourseOrder), typeof(PlayerCrawlOrder));
        }

        public void HideInCover()
        {
            _playerBrain.State.ChangeStance(CharacterStance.Sitting, GetOrderArguments());
            _playerBrain.ProcessSequence(GetOrderArguments(), typeof(PlayerCoverOrder));
        }

        public void LeaveCover()
        {
        }

        public void AttackEnemy()
        {
            if (CurrentEnemy == null)
            {
                return;
            }
            _playerBrain.State.ChangeStance(CharacterStance.Aiming, GetOrderArguments());
            _playerBrain.ProcessSequence(GetOrderArguments(), typeof(PlayerAimOrder));

            Shoot();

        }

        public void Shoot()
        {
            if(CurrentEnemy == null)
                return;
            
            _characterInventory.MainWeapon?.FireOnce(CurrentEnemy.GameObject.transform.position, Id);
        }

        public void Loot()
        {

        }

        void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag(TagsHelper.CoverTag))
            {
                var cover = other.GetComponent<Cover>();
                if (cover == _selectedCover)
                {
                    _selectedCover = null;
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