namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyIdleOrder : EnemyOrder
    {
        public EnemyIdleOrder(string name) : base(name)
        {
        }

        protected override void Activate(EnemyOrderArguments arguments)
        {
            arguments.Inventory.MainWeapon?.StopFiring();
            arguments.Navigator.LookOn(null);
        }

        protected override void Deactivate(EnemyOrderArguments arguments)
        {
        }

        protected override void Execute(EnemyOrderArguments arguments)
        {
        }
    }
}