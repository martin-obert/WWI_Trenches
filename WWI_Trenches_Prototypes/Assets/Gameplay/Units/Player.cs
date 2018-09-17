using System;
using System.Collections;
using Assets.Gameplay.Abstract;
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

    [RequireComponent(typeof(PlayerController), typeof(PlayerJumping))]
    public class Player : Singleton<Player>
    {
        [SerializeField] private Cover targetedCover;
        private Rigidbody[] rigidbodies;
        private PlayerController _controller;

        private PlayerJumping _jumping;

        public bool IsInCover { get; private set; }
        public bool IsJumping => _jumping.Playing;
        public bool IsAlive { get; private set; }

        private Vector3 _destination;

        [SerializeField] private Animator _animator;

        void Start()
        {
            //Todo: Presunout do jumping controlleru?
            var behaviors = _animator.GetBehaviours<CoverStateBehavior>();

            if (behaviors.Length > 0)
            {
                Debug.Log("Behaviors " + behaviors.Length);
                foreach (var customStateBehavior in behaviors)
                {
                    customStateBehavior.Player = this;
                }
            }

            rigidbodies = GetComponentsInChildren<Rigidbody>();

            ToggleRigid(false);

            _controller = GetComponent<PlayerController>();

            if (!_controller)
                Debug.LogError("No player controller component detected");

            _jumping = GetComponent<PlayerJumping>();

            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        public void TakeCover(Cover cover)
        {
            targetedCover = cover;
            Move(cover.transform.position);
        }

        private void Move(Vector3 target)
        {
            if (!IsAlive)
            {
                Debug.LogError("Cannot move with dead player");
                return;
            }

            _controller.Target = target;
        }

        public void Revive()
        {
            if (_controller)
                _controller.enabled = true;
            IsAlive = true;
        }

        public void Kill()
        {
            _controller.enabled = false;

            _jumping.Stop();

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

            _controller.NavMeshAgent.Warp(position);

            _controller.Target = destination;
        }
        public void JumpOver()
        {
            if (!IsJumping)
            {
                _controller.enabled = false;

                IsInCover = false;

                _jumping.Jump(targetedCover.JumpDestination.position, targetedCover.JumpDestination, targetedCover.PositionAdvanceCurve);
            }
        }

        private void JumpAnimationExited(CoverStateBehavior behavior)
        {
            IsInCover = false;

            _jumping.Stop();

            _controller.NavMeshAgent.Warp(transform.position);

            _controller.enabled = true;

            _controller.Target = _destination;

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
            if (other.gameObject.tag.Equals("Cover", StringComparison.CurrentCultureIgnoreCase))
            {
                var coverComponent = other.GetComponent<Cover>();
                if (!coverComponent)
                {
                    Debug.LogWarning("Trigger has no cover componenet");
                }
                else if (coverComponent == targetedCover)
                {
                    Debug.Log("Is in cover");

                    IsInCover = true;

                    _controller.NavMeshAgent.Warp(coverComponent.transform.position);

                    _controller.enabled = false;

                    transform.rotation = Quaternion.identity;
                    
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase))
            {
                var coverComponent = other.GetComponent<Cover>();
                if (coverComponent && coverComponent == targetedCover)
                {
                    Debug.Log("Is out of cover");

                    targetedCover = null;
                    IsInCover = false;
                }
            }
        }
    }
}
