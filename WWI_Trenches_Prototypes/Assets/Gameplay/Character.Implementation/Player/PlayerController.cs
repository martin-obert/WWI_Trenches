using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Gameplay.Abstract;
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

    public enum ThreatLevel
    {
        None,
        EnemyIsNear,
    }

    public class WeaponData : ScriptableObject
    {
        [SerializeField]
        private float _range = 1f;

        public string Name;

        [Tooltip("Attack frequency in seconds")]
        public float AttackSpeed = 0.5f;

        public float Range
        {
            get { return _range; }
        }
    }

    public class Weapon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private WeaponData _data;

        [SerializeField]
        private string _name;

        private ProxyZone _weaponRangeProxyZone;

        private GameObject _target;


        public bool IsStackable => false;

        public string Name => _name;

        public int MaxStack => 1;
        public int Amount => 1;

        public bool IsInRange { get; private set; }

        public bool CanFire => IsInRange;

        
        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                if (Mathf.Abs((_target.transform.position - transform.position).magnitude) <= Range)
                {
                    IsInRange = true;
                }
            }
        }

        public float Range => _data.Range;

        public float AttackSpeed => _data.AttackSpeed;

        public event EventHandler<IWeapon> IsInRangeChanged;

       
        void Awake()
        {
            _weaponRangeProxyZone.RangeRadius = _data.Range;
            _weaponRangeProxyZone.SubscribeTriggers(Inzone, Outzone);
        }

        void OnDisable()
        {
            StopFiring();
        }
        void OnDestroy()
        {
            StopFiring();
        }

        private void Outzone(object sender, EventArgs eventArgs)
        {
            if (Target && ((ProxyZone.ProxyZoneEvent)eventArgs).ZonedObject == Target)
            {
                IsInRange = false;
            }
        }

        private void Inzone(object sender, EventArgs eventArgs)
        {
            if (Target && ((ProxyZone.ProxyZoneEvent)eventArgs).ZonedObject == Target)
            {
                IsInRange = true;
            }
        }

        public void StartFiring()
        {
            InvokeRepeating(nameof(Fire), 0, AttackSpeed);
        }

        public void StopFiring()
        {
            CancelInvoke(nameof(Fire));
        }

        private void Fire()
        {
            print("Boom!");
        }
    }

    [RequireComponent(typeof(PlayerBrain), typeof(NavMeshAgent))]
    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField]
        private ProxyZone _enemyScanZone;

        private PlayerBrain _playerBrain;

        private PlayerCharacterState _playerCharacterState;

        public BasicCharacterAttributesContainer AttributesContainer { get; private set; }

        public PlayerCharacterNavigator Navigator { get; private set; }

        private PlayerInventory _playerInventory;

        //Todo: tohle se presune nekam
        public Vector3 Destination { get; set; }

        public PlayerState State => _playerCharacterState.CurrentState;

        public ProxyZone EnemyScanZone => _enemyScanZone;

        #region ToBe refactored

        public Weapon CurrentWeapon;

        public Enemy CurrentEnemy { get; private set; }
        #endregion

        void OnEnable()
        {
            AttributesContainer = new BasicCharacterAttributesContainer();

            _playerCharacterState = new PlayerCharacterState();

            _playerBrain = GetComponent<PlayerBrain>();

            _playerInventory = GetComponent<PlayerInventory>();

            Navigator = new PlayerCharacterNavigator(GetComponent<NavMeshAgent>(), AttributesContainer);

            if (_enemyScanZone)
            {
                _enemyScanZone.SubscribeTriggers(InZoneEventHandler, OutZoneEventHandler);
            }
        }

        private void OutZoneEventHandler(object sender, EventArgs eventArgs)
        {
            var ev = eventArgs as ProxyZone.ProxyZoneEvent;

            if (ev != null && ev.ZonedObject && ev.ZonedObject.CompareTag("Enemy"))
            {
                var enemy = ev.ZonedObject.GetComponent<Enemy>();
                if (CurrentEnemy == enemy)
                {
                    CurrentEnemy = null;
                }
            }
        }

        private void InZoneEventHandler(object sender, EventArgs eventArgs)
        {
            var ev = eventArgs as ProxyZone.ProxyZoneEvent;

            if (ev != null && ev.ZonedObject && ev.ZonedObject.CompareTag("Enemy"))
            {
                var enemy = ev.ZonedObject.GetComponent<Enemy>();
                if (!enemy)
                {
                    Debug.LogError("This is marked as enemy but has no enemy component" + ev.ZonedObject);
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

            _playerBrain.ChangeBehavior(this);
        }

        public void Run()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Running) return;

            print("Run");
            _playerCharacterState.ChangeState(PlayerState.Running, this);

            _playerBrain.ChangeBehavior(this);
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

        public void Attack()
        {
            if (_playerCharacterState.CurrentState == PlayerState.Attacking) return;

            if (!CurrentEnemy)
            {
                print("Player has no enemies");
                return;
            }

            if (!CurrentWeapon)
            {
                print("Player has no weapon");
                return;
            }

            CurrentWeapon.Target = CurrentEnemy.gameObject;

            print("Attacking");

            _playerCharacterState.ChangeState(PlayerState.Attacking, this);

            _playerBrain.ChangeBehavior(this);
        }

        public void FireWeapon()
        {
            InvokeRepeating(nameof(CurrentWeapon.StartFiring), 0, CurrentWeapon.AttackSpeed);
        }

        public void StopFiringWeapon()
        {
            CancelInvoke(nameof(CurrentWeapon.StartFiring));
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
