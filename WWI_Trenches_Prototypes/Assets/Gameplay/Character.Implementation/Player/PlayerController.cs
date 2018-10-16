using System;
using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using Assets.Gameplay.Units.Enemy;
using Assets.Gameplay.Zoning;
using Assets.TileGenerator;
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
    }

    public enum ThreatLevel
    {
        None,
        EnemyIsNear,
    }

    [RequireComponent(typeof(PlayerBrain), typeof(NavMeshAgent))]
    public class PlayerController : Singleton<PlayerController>, ITargetable
    {
        [SerializeField] private ProxyZone _enemyScanZone;

        private PlayerBrain _playerBrain;

        private PlayerCharacterState _playerCharacterState;

        public BasicCharacterAttributesContainer AttributesContainer { get; private set; }

        public PlayerCharacterNavigator Navigator { get; private set; }

        private CharacterInventory _characterInventory;

        //Todo: tohle se presune nekam
        public Vector3 Destination { get; set; }

        public PlayerState State => _playerCharacterState.CurrentState;

        public ProxyZone EnemyScanZone => _enemyScanZone;

        public string DisplayName => name;

        public GameObject GameObject => gameObject;

        public ITargetable CurrentEnemy { get; private set; }

        //Todo: Invoke from death method
        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        public void GotKilledBy(ITargetable killer)
        {
            throw new NotImplementedException();
        }

        void OnEnable()
        {
            AttributesContainer = new BasicCharacterAttributesContainer();

            _playerCharacterState = new PlayerCharacterState();

            _playerBrain = GetComponent<PlayerBrain>();

            _characterInventory = GetComponent<CharacterInventory>();

            Navigator = new PlayerCharacterNavigator(GetComponent<NavMeshAgent>(), AttributesContainer);

            if (_enemyScanZone)
            {
                _enemyScanZone.SubscribeTriggers(InZoneEventHandler, OutZoneEventHandler);
            }
        }

        private void OutZoneEventHandler(object sender, ProxyZone.ProxyZoneEvent eventArgs)
        {
            if (eventArgs != null && eventArgs.ZonedObject && eventArgs.ZonedObject.CompareTag(TagsHelper.EnemyTag))
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
            if (eventArgs != null && eventArgs.ZonedObject && eventArgs.ZonedObject.CompareTag(TagsHelper.EnemyTag))
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

        void OnDestroy()
        {
            if (_enemyScanZone)
            {
                _enemyScanZone.UnsubscribeTriggers(InZoneEventHandler, OutZoneEventHandler);
            }

            GCSingleton(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Crawl();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Run();
            }
        }

        private void Crawl()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Crawling) return;

            print("Crawl");
            _playerCharacterState.ChangeState(PlayerState.Crawling, this);

            _playerBrain.GiveOrder(this);
        }

        public void Run()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Running) return;

            print("Run");
            _playerCharacterState.ChangeState(PlayerState.Running, this);

            _playerBrain.GiveOrder(this);
        }

        public void Stop()
        {
        }

        public void TakeCover()
        {
        }

        public void LeaveCover()
        {
        }

        public void Shoot()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Shooting) return;

            print("Shooting");
            
            _playerCharacterState.ChangeState(PlayerState.Shooting, this);

            _playerBrain.GiveOrder(this);
        }

        public void Loot()
        {

        }

        public void Spawn(Vector3 startPointPosition)
        {
            Navigator.Teleport(startPointPosition);
        }
    }
}