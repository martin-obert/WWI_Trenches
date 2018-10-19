using System;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player
{
    [Obsolete("User character navigator")]
    public class PlayerCharacterNavigator : ICharacterNavigator<PlayerController>
    {
        public void LookOn(Transform target)
        {
            throw new NotImplementedException();
        }

        public void LookAtDirection(Quaternion roatation)
        {
            throw new NotImplementedException();
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

        public Vector3? Destination { get; }
    }
}