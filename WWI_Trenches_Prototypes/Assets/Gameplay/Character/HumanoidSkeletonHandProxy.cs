using Assets.Gameplay.Attributes;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    //Todo: create editor menu
    public class HumanoidSkeletonHandProxy : MonoBehaviour, IHumanoidSkeletonHandProxy
    {
        [SerializeField]
        private Transform _palm;

        [SerializeField]
        private Transform _ikTarget;

        public Transform Palm => _palm;

        public Transform IKTarget => _ikTarget;

        public BasicAttribute<float> PositionWeight { get; } = new BasicAttribute<float>("position_hand_ik", "Position Hand IK", 0, 0, 1);

        public BasicAttribute<float> RotationWeight { get; } = new BasicAttribute<float>("rotation_hand_ik", "Transforms Hand IK", 0, 0, 1);
    }
}