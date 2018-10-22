﻿using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface ICharacterNavigator<T>
    {
        void LookOn(Transform target);
        void LookAtDirection(Quaternion roatation);
        void Teleport(Vector3 position);
        void Move(Vector3? position);
        void Stop();
        void Disable();
        void Enable();
        Vector3? Destination { get;  }
    }
}