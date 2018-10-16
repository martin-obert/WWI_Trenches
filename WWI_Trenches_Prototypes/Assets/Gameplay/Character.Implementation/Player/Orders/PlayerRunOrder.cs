using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerRunOrder : PlayerOrder
    {
        private float _runSpeed = 5f;

        protected override void Activate(PlayerOrderArguments arguments)
        {

        }

        public override void Deactivate(PlayerOrderArguments arguments)
        {

        }

        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
            Debug.Log("running");
            arguments.Navigator.Move(arguments.Destination);
            arguments.Attributes.Speed.CurrentValue = _runSpeed;
        }

        public PlayerRunOrder(string name) : base(name)
        {
        }
    }
}