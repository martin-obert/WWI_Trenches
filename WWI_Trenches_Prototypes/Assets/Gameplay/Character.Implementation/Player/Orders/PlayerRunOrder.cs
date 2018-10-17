using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerRunOrder : PlayerOrder
    {
        private float _runSpeed = 5f;

       

        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
            arguments.Navigator.Enable();
            arguments.Navigator.Move(arguments.Destination);
            arguments.Attributes.Speed.CurrentValue = _runSpeed;
        }

        public PlayerRunOrder(string name) : base(name)
        {
        }
    }
}