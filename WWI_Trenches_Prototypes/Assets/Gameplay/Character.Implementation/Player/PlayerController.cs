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

        public int Id => GetInstanceID();

        public GameObject GameObject => gameObject;

        public ITargetable CurrentEnemy { get; private set; }

        //Todo: Invoke from death method
        public event EventHandler<ITargetable> EliminatedByOtherTarget;

        private Cover _selectedCover;

        public void GotKilledBy(ITargetable killer)
        {
            EliminatedByOtherTarget?.Invoke(this, killer);
        }

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
            CreateSingleton(this);
        }

        void OnDestroy()
        {
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

        private void Crawl()
        {
        }

        public void Run()
        {
        }

        public void Stop()
        {
        }

        public void TakeCover(Cover cover)
        {
            if (_playerBrain.State.CurrentStance == CharacterStance.Sitting && cover == _selectedCover) return;

            print("Cover");

            _selectedCover = cover;

            Destination = cover.transform.position;

            _playerBrain.State.ChangeStance(CharacterStance.Running, GetOrderArguments());
        }

        public void HideInCover()
        {
            _playerBrain.State.ChangeStance(CharacterStance.Sitting, GetOrderArguments());
        }

        public void LeaveCover()
        {
        }

        public void AttackEnemy()
        {
            
            if (_playerBrain.State.CurrentStance == CharacterStance.Aiming) return;

            print("Shooting");

            _playerBrain.State.ChangeStance(CharacterStance.Aiming, GetOrderArguments());

            Shoot();

        }

        public void Shoot()
        {
            if(CurrentEnemy == null)
                return;

            _characterInventory.MainWeapon?.StartFiring(CurrentEnemy.GameObject.transform.position, Id);
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
                    HideInCover();

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
            throw new NotImplementedException();
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