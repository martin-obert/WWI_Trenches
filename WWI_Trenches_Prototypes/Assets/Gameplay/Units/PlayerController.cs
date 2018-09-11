using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent _navAgent;
        private Vector3 _target;

        public Vector3 Target
        {
            get { return _target; }
            set
            {
                if(value.z <= transform.position.z) return;

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
            //Todo: tohle checknout at se nezacne jen tak hybat
            _navAgent.isStopped = false;
        }

        void OnDisable()
        {
            _navAgent.isStopped = true;
        }
    }
}