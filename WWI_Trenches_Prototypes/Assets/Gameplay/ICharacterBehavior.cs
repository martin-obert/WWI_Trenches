using System.Collections.Generic;
using Assets.Gameplay.Units;
using UnityEngine;

namespace Assets.Gameplay
{
    public interface IBehaviorArguments<TCharacter>
    {
        ICharacterNavigator<TCharacter> Navigator { get; }
    }

    public class PlayerBehaviorArguments : IBehaviorArguments<Player>
    {
        public ICharacterNavigator<Player> Navigator { get;  }

        public Vector3 Destination { get; }

        public Animator Animator { get; }
        public BasicCharacterAttributesContainer Attributes { get;  }

        public PlayerBehaviorArguments(Player player)
        {
            Animator = player.GetComponentInChildren<Animator>();
            Navigator = player.Navigator;
            Destination = player.Destination;
            Attributes = player.AttributesContainer;
        }
    }


    public interface ICharacterBehavior<TCharacter>
    {
        void Activate(IBehaviorArguments<TCharacter> arguments);
        void Deactivate(IBehaviorArguments<TCharacter> arguments);
        void Execute(IBehaviorArguments<TCharacter> arguments);
        string Name { get;  }
    }
}