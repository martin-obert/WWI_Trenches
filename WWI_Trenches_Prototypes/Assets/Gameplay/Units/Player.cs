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

    [RequireComponent(typeof(PlayerNavigationController), typeof(PlayerJumpingController))]
    public class Player : Singleton<Player>
    {
        [SerializeField] private Cover targetedCover;
        private Rigidbody[] rigidbodies;
        private PlayerNavigationController _navigationController;

        private PlayerJumpingController _jumpingController;

        public bool IsInCover { get; private set; }
        public bool IsJumping => _jumpingController.Playing;
        public bool IsAlive { get; private set; }

        private Vector3 _destination;

        [SerializeField] private Animator _animator;
        private Cover _currentCover;

        void Start()
        {
            _jumpingController = GetComponent<PlayerJumpingController>();
            _navigationController = GetComponent<PlayerNavigationController>();

            rigidbodies = GetComponentsInChildren<Rigidbody>();

            var behaviors = _animator.GetBehaviours<CoverStateBehavior>();

            if (behaviors.Length > 0)
            {
                foreach (var customStateBehavior in behaviors)
                {
                    customStateBehavior.Player = this;
                }
            }

            ToggleRigid(false);

            CreateSingleton(this, Instancing.None);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        public void TakeCover(Cover cover)
        {
            if (cover == _currentCover)
                return;

            if (IsInCover || IsJumping)
                SwapCover(cover);
            else
            {
                targetedCover = cover;
                Move(cover.transform.position);
            }
        }

        public void SwapCover(Cover cover)
        {
            if (!IsInCover || !cover || !(cover.transform.position.z > targetedCover.transform.position.z)) return;

            Debug.Log("Swaping cover");

            JumpOver();

            targetedCover = cover;
        }

        private void Move(Vector3 target)
        {
            if (IsInCover)
            {
                Debug.LogError("Cannot move while in cover!");
                return;
            }
            if (IsJumping)
            {
                Debug.LogError("Cannot move in jumping sequence!");
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

            _jumpingController.Stop();

            IsInCover = false;

            IsAlive = false;

            ToggleRigid(true);
        }


        private void ToggleRigid(bool value)
        {
            foreach (var rigid in rigidbodies)
            {
                rigid.useGravity = value;
                rigid.isKinematic = !value;
            }
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

            _navigationController.NavMeshAgent.Warp(position);

            _navigationController.Target = destination;
        }

        public void JumpOver()
        {
            if (!IsJumping && _currentCover)
            {
                _navigationController.enabled = false;

                IsInCover = false;

                _jumpingController.Jump(targetedCover.JumpDestination);
            }
        }

        private void JumpAnimationExited(CoverStateBehavior behavior)
        {
            IsInCover = false;

            _jumpingController.Stop();


            _navigationController.NavMeshAgent.Warp(transform.position);

            _navigationController.enabled = true;

            if (targetedCover && targetedCover.transform.position.z >= transform.position.z)
            {
                TakeCover(targetedCover);
            }
            else
            {
                Move(_destination);
            }
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JumpOver();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Kill();
            }
        }

        void LateUpdate()
        {
            if (_animator)
            {
                _animator.SetBool("IsInCover", IsInCover);
                _animator.SetBool("IsJumping", IsJumping);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.tag.Equals("Cover", StringComparison.CurrentCultureIgnoreCase)) return;

            var coverComponent = other.GetComponent<Cover>();

            if (coverComponent != targetedCover) return;

            Debug.Log("Is in cover");

            IsInCover = true;

            _currentCover = coverComponent;

            _navigationController.NavMeshAgent.Warp(coverComponent.transform.position);

            _navigationController.enabled = false;

            transform.rotation = Quaternion.identity;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase)) return;

            var coverComponent = other.GetComponent<Cover>();

            if (coverComponent != _currentCover) return;

            Debug.Log("Is out of cover");

            _currentCover = null;

            IsInCover = false;
        }
    }
}
