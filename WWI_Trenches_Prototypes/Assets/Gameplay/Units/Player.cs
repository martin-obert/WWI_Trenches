using System;
using System.Collections;
using Assets.Gameplay.Abstract;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Gameplay.Units
{
    public static class PlayerHelpers
    {
        public static Vector3 GetEndPoint(Player player, TiledTerrain terrain)
        {
            return new Vector3(player.transform.position.x, terrain.EndPoint.position.y, terrain.EndPoint.position.z);
        }

    }

    public enum ThreatLevel
    {
        None,
        EnemyIsNear,
    }

    public class PlayerStateChangedEvent : UnityEvent<Player>
    {

    }

    [RequireComponent(typeof(PlayerNavigationController))]
    public class Player : Singleton<Player>
    {

        [SerializeField] private Cover _targetedCover;

        private PlayerNavigationController _navigationController;

        public Enemy.Enemy CurrentEnemy { get; set; }

        public bool IsInCover
        {
            get { return _isInCover; }
            set
            {
                _isInCover = value;
                PlayerStateChanged.Invoke(this);
            }
        }
        public bool IsAttacking { get; private set; }

        public bool IsAlive { get; private set; }

        public bool IsCrawling { get; set; }

        public bool IsRunning { get; private set; }

        public float ThreatLevel
        {
            get { return _threatLevel; }
            set
            {
                _threatLevel = value;
                PlayerStateChanged.Invoke(this);
            }
        }

        public PlayerStateChangedEvent PlayerStateChanged { get; } = new PlayerStateChangedEvent();

        private Vector3 _destination;

        private Animator _animator;

        private Cover _currentCover;

        void Start()
        {
            _navigationController = GetComponent<PlayerNavigationController>();

            _animator = GetComponent<Animator>();

            if (!_animator)
                _animator = GetComponentInChildren<Animator>();

            var behaviors = _animator.GetBehaviours<MovementBehavior>();

            foreach (var movementBehavior in behaviors)
            {
                movementBehavior.PlayerInstance = this;
            }

            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        public void TakeCover(Cover cover)
        {
            if (cover == _currentCover)
                return;

            if (IsInCover)
                SwapCover(cover);
            else
            {
                _targetedCover = cover;
                Move(cover.transform.position);
            }
            //Crawl();
        }

        public void SwapCover(Cover cover)
        {
            if (!cover || !(cover.transform.position.z > _targetedCover.transform.position.z)) return;


            Debug.Log("Swaping cover");

            LeaveCover();

            _targetedCover = cover;

            Move(cover.transform.position);

        }

        public void LeaveCover()
        {
            if (!IsInCover) return;

            IsInCover = false;

            _navigationController.enabled = true;

            Run();
        }

        public void RunToEnd()
        {
            LeaveCover();
            Move(PlayerHelpers.GetEndPoint(this, TerrainManager.Instance.CurrentTerrain));
        }

        public void Attack()
        {
            if (IsAttacking || !CurrentEnemy) return;

            IsAttacking = true;

            LeaveCover();

            Move(CurrentEnemy.transform.position);

            StartCoroutine(CheckRange());
        }

        private IEnumerator CheckRange()
        {
            while (CurrentEnemy && (CurrentEnemy.transform.position - transform.position).magnitude > 1)
            {
                yield return null;
            }

            IsAttacking = false;

            Destroy(CurrentEnemy.gameObject);

            RunToEnd();
        }

        private void Move(Vector3 target)
        {
            if (IsInCover)
            {
                Debug.LogError("Cannot move while in cover!");
                return;
            }

            if (!IsAlive)
            {
                Debug.LogError("Cannot move with dead player");
                return;
            }

            _navigationController.Target = target;
        }

        public void Revive()
        {
            if (_navigationController)
                _navigationController.enabled = true;

            IsAlive = true;
        }

        public void Kill()
        {
            _navigationController.enabled = false;

            IsInCover = false;

            IsAlive = false;
        }

        public void Spawn(Vector3 position, Vector3? destination)
        {
            transform.position = position;

            if (destination.HasValue)
            {
                _destination = destination.Value;

                transform.rotation = Quaternion.FromToRotation(new Vector3(position.x, 0, position.z), new Vector3(destination.Value.x, 0, destination.Value.z));

                StartCoroutine(DelayedMove(position, destination.Value));
            }
        }

        private IEnumerator DelayedMove(Vector3 position, Vector3 destination)
        {
            yield return new WaitForSecondsRealtime(1);

            Revive();
            Run();
            _navigationController.NavMeshAgent.Warp(position);
            _navigationController.NavMeshAgent.speed = 3f;
            _navigationController.Target = destination;
        }


        public bool Stopped = false;
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Kill();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                IsRunning = !IsRunning;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Stopped = true;

            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.tag.Equals("Cover", StringComparison.CurrentCultureIgnoreCase) || _targetedCover && other.gameObject != _targetedCover.gameObject) return;

            var coverComponent = other.GetComponent<Cover>();

            if (coverComponent != _targetedCover) return;

            Debug.Log("Is in cover");

            IsInCover = true;

            _currentCover = coverComponent;

            _navigationController.NavMeshAgent.Warp(coverComponent.transform.position);

            _navigationController.enabled = false;

            Stop();

            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase) || _targetedCover && other.gameObject != _targetedCover.gameObject) return;

            var coverComponent = other.GetComponent<Cover>();

            if (coverComponent != _currentCover) return;

            Debug.Log("Is out of cover");

            _currentCover = null;

            IsInCover = false;
        }

        public void Run()
        {
            IsRunning = true;
            IsCrawling = false;
        }

        private float _backSpeed;
        private float _threatLevel;
        private bool _isInCover;

        public void Crawl()
        {
            _backSpeed = _navigationController.NavMeshAgent.speed;
            _navigationController.NavMeshAgent.speed = 0.7f;
            IsRunning = false;
            IsCrawling = true;
        }

        public void Stop()
        {
            _navigationController.NavMeshAgent.speed = 5;
            IsRunning = false;
            IsCrawling = false;
        }
    }
}
