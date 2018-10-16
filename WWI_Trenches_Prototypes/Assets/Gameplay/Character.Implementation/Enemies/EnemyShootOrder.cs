namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyShootOrder : EnemyOrder
    {
        public EnemyShootOrder(string name) : base(name)
        {
        }

        protected override void Activate(EnemyOrderArguments arguments)
        {
            
        }

        protected override void Deactivate(EnemyOrderArguments arguments)
        {
            arguments.Inventory.MainWeapon?.StopFiring();
            
        }

        protected override void Execute(EnemyOrderArguments arguments)
        {
            arguments.Navigator.LookOn(arguments.Target.GameObject.transform);
            arguments.Inventory.MainWeapon?.StartFiring(arguments.Target.GameObject.transform.position);
        }
    }
}