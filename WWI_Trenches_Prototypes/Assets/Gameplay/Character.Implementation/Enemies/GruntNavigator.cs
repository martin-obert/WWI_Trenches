using System;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    [RequireComponent(typeof(LookAtConstraint))]
    public class GruntNavigator : MonoBehaviour, ICharacterNavigator<GruntController>
    {
        private LookAtConstraint _lookAtConstraint;

        void Awake()
        {
            _lookAtConstraint = GetComponent<LookAtConstraint>();
            _lookAtConstraint.AddSource(new ConstraintSource());
        }

        public void LookOn(Transform target)
        {
            if (!target)
            {
                _lookAtConstraint.enabled = false;
            }
            else
            {
                _lookAtConstraint.enabled = true;
                _lookAtConstraint.SetSource(0, new ConstraintSource
                {
                    sourceTransform = target,
                    weight = 1
                });
            }

        }

        public void Teleport(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void Move(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }
    }
}