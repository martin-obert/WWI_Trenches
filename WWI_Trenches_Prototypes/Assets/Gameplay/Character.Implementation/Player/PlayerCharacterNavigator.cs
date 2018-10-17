using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerCharacterNavigator : ICharacterNavigator<PlayerController>
    {
        private readonly NavMeshAgent _navMeshAgent;

        private readonly Transform _transform;

        public PlayerCharacterNavigator(NavMeshAgent navMeshAgent, BasicCharacterAttributesContainer attributes, Transform transform)
        {
            _navMeshAgent = navMeshAgent;
            _transform = transform;
            attributes.Speed.Subscribe(SpeedChanged);
        }

        private void SpeedChanged(object sender, float f)
        {
            if (_navMeshAgent)
            {
                _navMeshAgent.speed = f;
            }
        }

        public void LookOn(Transform target)
        {
            _transform.LookAt(target.position);
        }

        public void LookAtDirection(Quaternion roatation)
        {
            _transform.rotation = roatation;
        }

        public void Teleport(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Enable();
            }

            _navMeshAgent.Warp(position);
        }

        public void Move(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Enable();
            }

            _navMeshAgent.SetDestination(position);
        }

        public void Stop()
        {
            //Todo: asi resit jinak, neco jako speed 0 nebo tak pokud to bude hazet errory + nastudovat na gitu jak to vlastne funguje :)
            Disable();

        }

        public void Disable()
        {
            _navMeshAgent.enabled = false;
        }

        public void Enable()
        {

            _navMeshAgent.enabled = true;
        }

        private bool CheckNavMesh()
        {
            return _navMeshAgent;
        }
    }
}