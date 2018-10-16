using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerIdleOrder : PlayerOrder
    {
        protected override void Activate(PlayerOrderArguments arguments)
        {
        }

        public override void Deactivate(PlayerOrderArguments arguments)
        {
        }

        public override void Execute(PlayerOrderArguments arguments)
        {
            arguments.Navigator.Stop();
        }

        public PlayerIdleOrder(string name) : base(name)
        {
        }
    }
}