using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerCrawlOrder : PlayerOrder
    {
        private float _crawlSpeed = 0.3f;

        public PlayerCrawlOrder(string name) : base(name)
        {
        }

        protected override void Activate(PlayerOrderArguments arguments)
        {

        }

        public override void Deactivate(PlayerOrderArguments arguments)
        {

        }

        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, -1);

            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 0);

            arguments.Attributes.Speed.CurrentValue = _crawlSpeed;
        }
    }
}