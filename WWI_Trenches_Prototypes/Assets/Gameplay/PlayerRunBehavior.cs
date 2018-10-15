using UnityEngine;

namespace Assets.Gameplay
{
    public class PlayerRunBehavior : PlayerBehavior
    {
        private float _runSpeed = 5f;

        protected override void Activate(PlayerBehaviorArguments arguments)
        {
            
        }

        public override void Deactivate(PlayerBehaviorArguments arguments)
        {
            
        }

        public override void Execute(PlayerBehaviorArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
            Debug.Log("running");
            arguments.Navigator.Move(arguments.Destination);
            arguments.Attributes.Speed.Value(_runSpeed);
        }

        public PlayerRunBehavior(string name) : base(name)
        {
        }
    }
}