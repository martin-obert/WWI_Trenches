using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public abstract class PlayerOrder : ICharacterOrder<PlayerController>
    {
        protected PlayerOrder(string name)
        {
            Name = name;
        }

        public void Execute(IOrderArguments<PlayerController> arguments)
        {
            Execute((PlayerOrderArguments)arguments);
        }

        public string Name { get; }

        public void Execute<T>(IOrderArguments<T> arguments)
        {
            Execute((PlayerOrderArguments)arguments);
        }

        public abstract void Execute(PlayerOrderArguments arguments);
    }
    }