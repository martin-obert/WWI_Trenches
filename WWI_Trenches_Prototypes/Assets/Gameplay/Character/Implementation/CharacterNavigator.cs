﻿using UnityEngine;
using UnityEngine.AI;

namespace Assets.Gameplay.Character.Implementation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterNavigator : MonoBehaviour, ICharacterNavigator
    {
        private NavMeshAgent _navMeshAgent;

        [SerializeField, Tooltip("Root object of current character, if empty, then set to this transform")]
        private Transform _rootObject;

        [SerializeField]
        private NavigatorAttributes _attributes;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            if (!_rootObject)
                _rootObject = transform;

            _attributes.Speed.Subscribe(SpeedChanged);
        }

        void OnDestroy()
        {
            _attributes.Speed.Unsubscribe(SpeedChanged);
        }

        private void SpeedChanged(object sender, float f)
        {
            _navMeshAgent.speed = f;
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
            print("Nav mesh destination " + Destination);
            if (!position.HasValue)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public void Stop()
        {
            //Todo: bacha na speed = 0, eventem se divam na speed value attributu
            Disable();

        }

        public void Disable()
        {
            _navMeshAgent.enabled = false;
        }

        public void Enable()
        {
            print("Enabling nav mesh");
            _navMeshAgent.enabled = true;

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