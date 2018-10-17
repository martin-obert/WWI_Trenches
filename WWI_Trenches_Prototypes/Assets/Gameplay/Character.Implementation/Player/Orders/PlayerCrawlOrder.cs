using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerChangeCourseOrder : PlayerOrder
    {
        public PlayerChangeCourseOrder(string name) : base(name)
        {
        }

     
        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Navigator.Enable();
            arguments.Navigator.Move(arguments.Destination);
        }
    }

    public class PlayerCrawlOrder : PlayerOrder
    {
        private float _crawlSpeed = 0.3f;

        public PlayerCrawlOrder(string name) : base(name)
        {
        }

        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendXHandle, -1);

            arguments.Animator.SetFloat(PlayerAnimatorParameter.BlendYHandle, 0);
            arguments.Navigator.Enable();
            arguments.Navigator.Move(arguments.Destination);
            arguments.Attributes.Speed.CurrentValue = _crawlSpeed;
        }
    }
}