using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Implementation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterNavigator : MonoBehaviour, ICharacterNavigator
    {
        private NavMeshAgent _navMeshAgent;

        [SerializeField, Tooltip("Root object of current character, if empty, then set to this transform")]
        private Transform _rootObject;

        private float internalSpeed = 0;
        [SerializeField]
        private NavigatorAttributes _attributes;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            if (!_rootObject)
                _rootObject = transform;

            _attributes.Speed.Subscribe(SpeedChanged);
            SpeedChanged(null, _attributes.Speed.Value());
        }

        void OnDestroy()
        {
            _attributes.Speed.Unsubscribe(SpeedChanged);
        }

        private void SpeedChanged(object sender, float f)
        {
            _navMeshAgent.speed = internalSpeed = f;
        }

        public void LockOn(Transform target)
        {
            
            _rootObject.LookAt(target.position);
        }

        public void LookAtDirection(Quaternion roatation)
        {
            _rootObject.rotation = roatation;
        }

        public void Teleport(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Enable();
            }

            _navMeshAgent.Warp(position);
        }

        public void Move(Vector3? position)
        {
            Destination = position;
            if (!position.HasValue)
            {
                Disable();
            }
            else
            {
                Enable();
                if (_navMeshAgent.destination != Destination.Value)
                    _navMeshAgent.SetDestination(Destination.Value);
            }
        }

        public void Stop()
        {
            //Todo: bacha na speed = 0, eventem se divam na speed value attributu
            Disable();

        }

        public void Disable()
        {
            if (!_navMeshAgent.enabled) return;
            _navMeshAgent.speed = 0;
            _navMeshAgent.enabled = false;
        }

        public void Enable()
        {
            if (_navMeshAgent.enabled) return;

            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = internalSpeed;
            if (Destination.HasValue && _navMeshAgent.destination != Destination.Value)
                _navMeshAgent.destination = Destination.Value;
        }

        public void Continue()
        {
            Enable();
        }

        private bool CheckNavMesh()
        {
            return _navMeshAgent;
        }

        public Vector3? Destination { get; private set; }
    }
}