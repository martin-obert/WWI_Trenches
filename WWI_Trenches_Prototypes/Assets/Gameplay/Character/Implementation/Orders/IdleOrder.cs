
namespace Assets.Gameplay.Character.Implementation.Orders
{
    public class IdleOrder : Order
    {
        public override void Execute(CharacterOrderArguments arguments)
        {
            arguments.Navigator.Stop();
        }

        public IdleOrder(string name) : base(name)
        {
        }
    }
}