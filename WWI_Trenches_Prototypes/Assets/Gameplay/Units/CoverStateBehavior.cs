using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class CoverStateBehavior : StateMachineBehaviour
    {
        public Player Player;

        //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    if (Player && Player.IsInCover && !Player.IsJumping  &&  stateInfo.IsName("Jumping"))
        //    {

        //    }
        //    base.OnStateEnter(animator, stateInfo, layerIndex);
        //}

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Player &&  stateInfo.IsName("Armature|Jump"))
            {
                Debug.Log("State changed to non Jump");
                Player.SendMessage("JumpAnimationExited", this, SendMessageOptions.DontRequireReceiver);
            }
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }
}