using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public abstract class EnemyOrder : ICharacterOrder<GruntController>
    {
        public string Name { get; }

        protected EnemyOrder(string name)
        {
            Name = name;
        }

        protected abstract void Execute(EnemyOrderArguments arguments);

        public void Execute(IOrderArguments<GruntController> arguments)
        {
            Execute((EnemyOrderArguments)arguments);
        }

        public void Execute<T>(IOrderArguments<T> arguments)
        {
            Execute((EnemyOrderArguments)arguments);
        }
    }
}