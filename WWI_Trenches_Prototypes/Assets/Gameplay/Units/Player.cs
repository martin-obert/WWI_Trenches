using System.Collections;
using Assets.Gameplay.Abstract;
using UnityEngine;

namespace Assets.Gameplay.Units
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Singleton<Player>
    {
        private PlayerController _controller;


        public bool IsAlive { get; private set; }

        void Awake()
        {
            _controller = GetComponent<PlayerController>();
        }

        void Start()
        {
            CreateSingleton(this);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        public void Move(Vector3 target)
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
                transform.rotation = Quaternion.FromToRotation(new Vector3(position.x, 0, position.z), new Vector3(destination.Value.x, 0, destination.Value.z));
                StartCoroutine(DelayedMove(destination.Value));
            }
        }

        private IEnumerator DelayedMove(Vector3 destination)
        {
            yield return new WaitForSecondsRealtime(1);

            Move(destination);
        }
    }
}
