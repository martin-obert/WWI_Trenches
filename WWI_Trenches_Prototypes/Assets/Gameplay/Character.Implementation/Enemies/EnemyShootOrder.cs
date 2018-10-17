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
            
        }

        protected override void Execute(EnemyOrderArguments arguments)
        {
            arguments.Navigator.LookOn(arguments.CurrentTarget.GameObject.transform);
        }
    }
}