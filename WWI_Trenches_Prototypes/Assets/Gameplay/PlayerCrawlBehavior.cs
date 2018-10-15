namespace Assets.Gameplay
{
    public class PlayerCrawlBehavior : PlayerBehavior
    {
        private float _crawlSpeed = 0.3f;

        protected override void Activate(PlayerBehaviorArguments arguments)
        {

        }

        public override void Deactivate(PlayerBehaviorArguments arguments)
        {

        }

        public override void Execute(PlayerBehaviorArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, 0);
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 0);
            arguments.Attributes.Speed.Value(_crawlSpeed);
        }

        public PlayerCrawlBehavior(string name) : base(name)
        {
        }
    }
}