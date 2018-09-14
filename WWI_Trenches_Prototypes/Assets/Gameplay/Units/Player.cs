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

    [RequireComponent(typeof(PlayerController))]
    public class Player : Singleton<Player>
    {
        private Cover targetedCover;
        private PlayerController _controller;
        
        public bool IsInCover { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsAlive { get; private set; }
        private Vector3 _destination;
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
         
        }

        void Start()
        {
            var behaviors = GetComponent<Animator>().GetBehaviours<CustomStateBehavior>();
            if (behaviors.Length > 0)
            {
                foreach (var customStateBehavior in behaviors)
                {
                    customStateBehavior.Player = this;
                }
            }
                
                
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
            _controller.enabled = true;
            IsAlive = true;
        }

        public void Kill()
        {
            _controller.enabled = false;
            IsAlive = false;
        }

        public void Spawn(Vector3 position, Vector3? destination)
        {
            Revive();

            transform.position = position;

            if (destination.HasValue)
            {
                _destination = destination.Value;
                transform.rotation = Quaternion.FromToRotation(new Vector3(position.x, 0, position.z), new Vector3(destination.Value.x, 0, destination.Value.z));
                StartCoroutine(DelayedMove(position, destination.Value));
            }
        }

        private IEnumerator DelayedMove(Vector3 position,Vector3 destination)
        {
            Debug.Log("Warping");
            _controller.NavMeshAgent.Warp(position);
            yield return new WaitForSecondsRealtime(1);
            
            
            _controller.Target = destination;
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        IsInCover = true;
        //        _controller.enabled = false;
        //        Debug.Log("Is in cover");
        //    }
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    if (collision.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        IsInCover = false;
        //        _controller.enabled = true;
        //        Debug.Log("Is out of cover");
        //    }
        //}

        void Update()
        {
            if ( Input.GetKeyDown(KeyCode.Space))
            {
                IsJumping = true;
                GetComponent<Animator>().SetBool("IsJumping", IsJumping);
            }
        }

        public void JumpAnimationEnded()
        {
            IsJumping = false;
            GetComponent<Animator>().SetBool("IsJumping", IsJumping);
            
        }

        private Vector3 _jumpPoint;
        private float _jumpSpeed = 1f;
        private float _jumpTreshold = 0.0f;

        public void JumpOver()
        {
            if (IsInCover)
            {
                _controller.enabled = false;
                _jumpPoint = targetedCover.JumpDestination.position;
                IsInCover = false;
                IsJumping = true;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("cover", StringComparison.CurrentCultureIgnoreCase))
            {
                var coverComponent = other.GetComponent<Cover>();
                if (!coverComponent)
                {
                    Debug.LogWarning("Trigger has no cover componenet");
                }
                else if(coverComponent == targetedCover)
                {
                    Debug.Log("Is in cover");
                    IsInCover = true;
                    _controller.enabled = false;
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
