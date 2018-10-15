using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class MovementBehavior : StateMachineBehaviour
    {
        public const string BlendXParam = "BlendX";
        public const string BlendYParam = "BlendY";
        public const string StopParam = "HasStopped";
        public const string CoverParam = "IsInCover";

        private int _blendX = Animator.StringToHash(BlendXParam);
        private int _blendY = Animator.StringToHash(BlendYParam);
        private int _hasStopped = Animator.StringToHash(StopParam);
        private int _isInCover = Animator.StringToHash(CoverParam);

        public Player PlayerInstance;

        private float _blendXVal;
        private float _blendYVal;

        public float BlendSpeed = 0.1f;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PlayerInstance)
            {
                //if (PlayerInstance.IsRunning)
                //{
                //    _blendYVal = 1;
                //    _blendXVal = 0;
                //}
                //else if (PlayerInstance.IsCrawling)
                //{
                //    _blendYVal = 0;
                //    _blendXVal = -1;
                //}
                //else if (PlayerInstance.IsInCover)
                //{
                //    _blendXVal = 0;
                //    _blendYVal = -1;
                //}

                animator.SetFloat(_blendY, _blendYVal);
                animator.SetFloat(_blendX, _blendXVal);
            }
        }
    }
}