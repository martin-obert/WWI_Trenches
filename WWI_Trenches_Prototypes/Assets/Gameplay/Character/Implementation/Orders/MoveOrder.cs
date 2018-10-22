using Assets.Gameplay.Character.Implementation.Player;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Orders
{
    [CreateAssetMenu(menuName = "Character/Basic/Move order", fileName = "Move Order")]
    public class MoveOrder : Order
    {
        [SerializeField, Range(0, 100), Tooltip("Max speed %")]
        private float _speedPercentace = 50f;

        public override void Execute(CharacterOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 1);
            arguments.Attributes.Speed.Value((float)arguments.Attributes.Speed.MaxValue * (_speedPercentace / 100f));
        }

        public MoveOrder(string name) : base(name)
        {
        }
    }
}