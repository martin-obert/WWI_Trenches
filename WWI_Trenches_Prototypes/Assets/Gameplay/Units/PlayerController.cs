using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent _navAgent;
        private Vector3 _target;
        public NavMeshAgent NavMeshAgent => _navAgent;
        public Vector3 Target
        {
            get { return _target; }
            set
            {
                if (value.z <= transform.position.z) return;

                _target = value;
                _navAgent.SetDestination(_target);
            }
        }

        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
        }

        void OnEnable()
        {
            _navAgent.enabled = true;
        }

        void OnDisable()
        {
            _navAgent.enabled = false;
        }
    }
}