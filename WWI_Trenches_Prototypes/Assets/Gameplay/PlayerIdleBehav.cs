using Assets.Gameplay.Units;

namespace Assets.Gameplay
{
    public class PlayerIdleBehav : PlayerBehavior
    {
        protected override void Activate(PlayerBehaviorArguments arguments)
        {
        }

        public override void Deactivate(PlayerBehaviorArguments arguments)
        {
        }

        public override void Execute(PlayerBehaviorArguments arguments)
        {
            arguments.Navigator.Stop();
        }

        public PlayerIdleBehav(string name) : base(name)
        {
        }
    }
}