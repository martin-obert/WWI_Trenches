using Assets.Gameplay.Character.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Implementation.Player
{
    public class PlayerCharacterNavigator : ICharacterNavigator<PlayerController>
    {
        private readonly NavMeshAgent _navMeshAgent;

        public PlayerCharacterNavigator(NavMeshAgent navMeshAgent, BasicCharacterAttributesContainer attributes)
        {
            _navMeshAgent = navMeshAgent;

            attributes.Speed.Subscribe(SpeedChanged);
        }

        private void SpeedChanged(float value)
        {
            if (_navMeshAgent)
            {
                _navMeshAgent.speed = value;
            }
        }

        public void Teleport(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Debug.LogError("Nav mesh agent is not active");
                return;
            }

            _navMeshAgent.Warp(position);
        }

        public void Move(Vector3 position)
        {
            if (!CheckNavMesh())
            {
                Debug.LogError("Nav mesh agent is not active");
                return;
            }

            _navMeshAgent.destination = position;
        }

        public void Stop()
        {
            if (!CheckNavMesh())
            {
                Debug.LogError("Nav mesh agent is not active");
                return;
            }

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