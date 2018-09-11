using UnityEngine;
using UnityEngine.AI;

namespace Assets.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour
    {
        private NavMeshAgent _navAgent;



        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
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
