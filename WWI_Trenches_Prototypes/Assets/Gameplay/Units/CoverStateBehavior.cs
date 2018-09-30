using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class CoverStateBehavior : StateMachineBehaviour
    {
        public Player Player;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }
}