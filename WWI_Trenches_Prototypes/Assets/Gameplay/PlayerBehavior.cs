using Assets.Gameplay.Units;

namespace Assets.Gameplay
{
    public abstract class PlayerBehavior : ICharacterBehavior<Player>
    {
        protected PlayerBehavior(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Activate(IBehaviorArguments<Player> arguments)
        {
            Activate((PlayerBehaviorArguments)arguments);
        }

        public void Deactivate(IBehaviorArguments<Player> arguments)
        {
            Deactivate((PlayerBehaviorArguments)arguments);
        }

        public void Execute(IBehaviorArguments<Player> arguments)
        {
            Execute((PlayerBehaviorArguments)arguments);
        }

        protected abstract void Activate(PlayerBehaviorArguments arguments);

        public abstract void Deactivate(PlayerBehaviorArguments arguments);

        public abstract void Execute(PlayerBehaviorArguments arguments);
    }
    }