using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class CustomStateBehavior : StateMachineBehaviour
    {
        public Player Player;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Player  &&  stateInfo.IsName("Idle"))
            {
                Debug.Log("Moving Player");
                Player.transform.position = Player.transform.position + Player.transform.forward * 5f;
            }
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Player && Player.IsJumping && stateInfo.IsName("Jumping"))
            {
                Player.JumpAnimationEnded();
            }
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }
}