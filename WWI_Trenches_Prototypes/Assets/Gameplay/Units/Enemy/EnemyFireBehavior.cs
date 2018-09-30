using System;
using UnityEngine;

namespace Assets.Gameplay.Units.Enemy
{
    [Obsolete]
    public class EnemyFireBehavior : StateMachineBehaviour
    {
        public const string FireState = "";

        private int _fireStateHandle = Animator.StringToHash(FireState);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.Play(_fireStateHandle);
            //base.OnStateEnter(animator, stateInfo, layerIndex);
        }
    }
}
