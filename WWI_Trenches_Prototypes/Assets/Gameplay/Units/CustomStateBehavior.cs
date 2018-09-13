using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class CustomStateBehavior : StateMachineBehaviour
    {
        public Player Player;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Idle"))
            {
                Debug.Log("Idle");
            }
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }
}