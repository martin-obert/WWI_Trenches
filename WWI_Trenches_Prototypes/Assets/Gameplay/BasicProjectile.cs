using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay
{
    public class BasicProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _lifeTime;

        public int ShotFromWeaponId { get; }

        public int ShotByCharacterId { get; }

        public float Speed => _speed;

        public float Lifetime => _lifeTime;

        public Vector3? StartingPosition { get; private set; }

        public Vector3? DirectionNorm { get; private set; }

        public bool IsFired { get; private set; }

        void Start()
        {
            ResetToStack();
        }

        public void Shoot(Vector3 fromPosition, Vector3 directionNorm)
        {
            if (IsFired)
            {
                Debug.LogError("Shooting fired projectile expand stack if neeeded");
                return;
            }
            //print("projectile");
            gameObject.SetActive(true);

            gameObject.transform.SetPositionAndRotation(fromPosition, Quaternion.LookRotation(directionNorm));

            Invoke(nameof(ResetToStack), _lifeTime);

            IsFired = true;
        }

        public void ResetToStack()
        {
            IsFired = false;

            transform.localPosition = Vector3.zero;

            gameObject.SetActive(false);

            StartingPosition = null;

            DirectionNorm = null;
        }

        void LateUpdate()
        {
            if (IsFired)
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * Speed);
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetInstanceID() == ShotByCharacterId) return;

            var trigger = other.gameObject.GetComponent<ITargetable>();

            trigger?.GotHitRanged(this);
        }
    }
}