using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public abstract class PlayerOrder : ICharacterOrder<PlayerController>
    {
        protected PlayerOrder(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Activate(IOrderArguments<PlayerController> arguments)
        {
            Activate((PlayerOrderArguments)arguments);
        }

        public void Deactivate(IOrderArguments<PlayerController> arguments)
        {
            Deactivate((PlayerOrderArguments)arguments);
        }

        public void Execute(IOrderArguments<PlayerController> arguments)
        {
            Execute((PlayerOrderArguments)arguments);
        }

        protected abstract void Activate(PlayerOrderArguments arguments);

        public abstract void Deactivate(PlayerOrderArguments arguments);

        public abstract void Execute(PlayerOrderArguments arguments);
    }
    }