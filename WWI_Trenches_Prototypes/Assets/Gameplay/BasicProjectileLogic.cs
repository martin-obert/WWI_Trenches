using Assets.Gameplay.Character;
using UnityEngine;

namespace Assets.Gameplay
{
    /// <summary>
    /// Expensive shitty projectileLogic
    /// </summary>
    public class BasicProjectileLogic : MonoBehaviour, IProjectileLogic
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _lifeTime;

        public int ShotByCharacterId { get; private set; }

        public float Damage { get; private set; }
        public float Speed => _speed;

        public float Lifetime => _lifeTime;
        public bool IsFired { get; private set; }
        public void RayCastShot(IIdentificable shooter, Vector3 fromPosition, Vector3 directionNorm, float damage)
        {
        
            if (IsFired)
            {
                Debug.LogError("Shooting fired projectileLogic expand stack if neeeded");
                return;
            }

            Damage = damage;
            IsFired = true;
            ShotByCharacterId = shooter.Id;

            print("projectileLogic");

            gameObject.SetActive(true);

            gameObject.transform.position = fromPosition;

            gameObject.transform.rotation = Quaternion.LookRotation(directionNorm);

            Invoke(nameof(ResetToStack), _lifeTime);

        }

        public void InstantShot(IIdentificable shooter, ITargetable target)
        {
            throw new System.NotImplementedException();
        }

        public void ResetToStack()
        {
            IsFired = false;

            transform.localPosition = Vector3.zero;

            gameObject.SetActive(false);
        }

        public GameObject GameObject => gameObject;

        void LateUpdate()
        {
            if (IsFired)
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * Speed);
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetInstanceID() == ShotByCharacterId)
            {
                return;
            }

            var trigger = other.gameObject.GetComponent<ITargetable>();

            if (trigger == null)
                return;

            trigger.GotHitRanged(this);
            ResetToStack();
        }
    }
}