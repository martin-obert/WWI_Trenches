using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private NavMeshAgent _navAgent;


        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            if (Instance && Instance != this)
            {
                Destroy(Instance);
            }

            Instance = this;
        }

        void Update()
        {
            
            if (Input.GetAxis("Fire2") > 0)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit))
                {
                    _navAgent.SetDestination(rayHit.point);
                }

            }
        }

    }
}
