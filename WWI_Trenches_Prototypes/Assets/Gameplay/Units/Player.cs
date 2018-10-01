using System;
using System.Collections;
using Assets.Gameplay.Abstract;
using Assets.IoC;
using Assets.TileGenerator;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Units
{
    public static class PlayerHelpers
    {
        public static Vector3 GetEndPoint(Player player, TiledTerrain terrain)
        {
            return new Vector3(player.transform.position.x, terrain.EndPoint.position.y, terrain.EndPoint.position.z);
        }

    }

    [RequireComponent(typeof(PlayerNavigationController))]
    public class Player : Singleton<Player>
    {

        [SerializeField] private Cover _targetedCover;

        private PlayerNavigationController _navigationController;

        public bool IsInCover { get; set; }

        public bool IsAlive { get; private set; }

        public bool IsCrawling { get; set; }



        public bool IsRunning { get; private set; }

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
            Crawl();
        }

        public void SwapCover(Cover cover)
        {
            if (!cover || !(cover.transform.position.z > _targetedCover.transform.position.z)) return;

            IsInCover = false;

            Debug.Log("Swaping cover");

            _targetedCover = cover;
           
            _navigationController.enabled = true;
            Move(cover.transform.position);
            Run();
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
            transform.rotation = Quaternion.Euler(0,180,0);
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
