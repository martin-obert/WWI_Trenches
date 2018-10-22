using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface ICharacterNavigator
    {
        void LockOn(Transform target);
        void LookAtDirection(Quaternion roatation);
        void Teleport(Vector3 position);
        void Move(Vector3? position);
        void Stop();
        void Disable();
        void Enable();
        void Continue();
        Vector3? Destination { get;  }
    }
}