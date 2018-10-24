using UnityEngine;

namespace Assets.Gameplay.Character
{
    [RequireComponent(typeof(Animator))]
    public class HumanoidSkeletonProxy : MonoBehaviour, IHumanoidSkeletonProxy
    {
        [SerializeField]
        private HumanoidSkeletonHandProxy _leftHand;

        [SerializeField]
        private HumanoidSkeletonHandProxy _rightHand;

        public IHumanoidSkeletonHandProxy LeftHandProxy => _leftHand;

        public IHumanoidSkeletonHandProxy RightHandProxy => _rightHand;

        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void OnAnimatorIK()
        {
            if (_rightHand != null && _rightHand.IKTarget)
            {
                UpdateHandIk(AvatarIKGoal.RightHand, _rightHand);
            }

            if (_leftHand != null && _leftHand.IKTarget)
            {
                UpdateHandIk(AvatarIKGoal.LeftHand,_leftHand);
            }
        }

        private void UpdateHandIk(AvatarIKGoal goal, IHumanoidSkeletonHandProxy hand)
        {
            _animator.SetIKPosition(goal, hand.IKTarget.position);
            _animator.SetIKRotation(goal, hand.IKTarget.rotation);
            _animator.SetIKPositionWeight(goal, hand.PositionWeight.Value());
            _animator.SetIKRotationWeight(goal, hand.RotationWeight.Value());
        }
    }
}