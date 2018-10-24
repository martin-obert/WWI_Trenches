using Assets.Gameplay.Attributes;
using Assets.Gameplay.Instructions;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface ICharacterBrain<TCharacter> : ISequenceExecutor
    {
        ICharacterMemory<TCharacter> Memory { get; }
    }

    public interface IHumanoidSkeletonProxy
    {
        IHumanoidSkeletonHandProxy LeftHandProxy { get; }
        IHumanoidSkeletonHandProxy RightHandProxy { get; }
    }

    public interface IHumanoidSkeletonHandProxy
    {
        Transform Palm { get; }

        Transform IKTarget { get; }

        BasicAttribute<float> PositionWeight { get; }

        BasicAttribute<float> RotationWeight { get; }
    }

    public class HumanoidSkeletonHandProxy : IHumanoidSkeletonHandProxy
    {
        [SerializeField]
        private Transform _palm;

        [SerializeField]
        private Transform _ikTarget;

        public Transform Palm => _palm;

        public Transform IKTarget => _ikTarget;

        public BasicAttribute<float> PositionWeight { get; } = new BasicAttribute<float>("position_hand_ik", "Position Hand IK", 0, 0, 1);

        public BasicAttribute<float> RotationWeight { get; } = new BasicAttribute<float>("rotation_hand_ik", "Rotation Hand IK", 0, 0, 1);
    }

    [RequireComponent(typeof(Animator))]
    public class HumanoidSkeletonProxy : MonoBehaviour, IHumanoidSkeletonProxy
    {
        [SerializeField]
        private IHumanoidSkeletonHandProxy _leftHand;

        [SerializeField]
        private IHumanoidSkeletonHandProxy _rightHand;

        public IHumanoidSkeletonHandProxy LeftHandProxy => _leftHand;

        public IHumanoidSkeletonHandProxy RightHandProxy => _rightHand;

        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void AnimatiorIK()
        {
            if (_rightHand != null && _rightHand.IKTarget)
            {
                UpdateHandIk(_rightHand);
            }

            if (_leftHand != null && _leftHand.IKTarget)
            {
                UpdateHandIk(_leftHand);
            }
        }

        private void UpdateHandIk(IHumanoidSkeletonHandProxy hand)
        {
            _animator.SetIKPosition(AvatarIKGoal.RightHand, hand.IKTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, hand.IKTarget.rotation);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, hand.PositionWeight.Value());
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, hand.RotationWeight.Value());
        }
    }
}
