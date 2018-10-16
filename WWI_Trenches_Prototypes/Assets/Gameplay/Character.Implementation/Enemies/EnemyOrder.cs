using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public abstract class EnemyOrder : ICharacterOrder<GruntController>
    {
        protected EnemyOrder(string name)
        {
            Name = name;
        }

        public void Activate(IOrderArguments<GruntController> arguments)
        {
            Activate((EnemyOrderArguments)arguments);
        }

        public void Deactivate(IOrderArguments<GruntController> arguments)
        {
            Deactivate((EnemyOrderArguments)arguments);
        }

        public void Execute(IOrderArguments<GruntController> arguments)
        {
            Execute((EnemyOrderArguments)arguments);
        }

        protected abstract void Activate(EnemyOrderArguments arguments);
        protected abstract void Deactivate(EnemyOrderArguments arguments);
        protected abstract void Execute(EnemyOrderArguments arguments);

        public string Name { get; }
    }
}