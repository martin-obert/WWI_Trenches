using Assets.Gameplay.Character.Implementation.Player;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface IOrderArguments<TCharacter>
    {
        ICharacterNavigator<TCharacter> Navigator { get; }
    }

    public class PlayerOrderArguments : IOrderArguments<PlayerController>
    {
        public ICharacterNavigator<PlayerController> Navigator { get;  }

        public Vector3 Destination { get; }

        public Animator Animator { get; }
        public BasicCharacterAttributesContainer Attributes { get;  }

        public PlayerInventory Inventory { get; }
        public PlayerOrderArguments(PlayerController player)
        {
            Animator = player.GetComponentInChildren<Animator>();
            Navigator = player.Navigator;
            Destination = player.Destination;
            Attributes = player.AttributesContainer;

            //Todo: backing field
            Inventory = player.GetComponent<PlayerInventory>();
        }
    }


    public interface ICharacterOrder<TCharacter>
    {
        void Activate(IOrderArguments<TCharacter> arguments);
        void Deactivate(IOrderArguments<TCharacter> arguments);
        void Execute(IOrderArguments<TCharacter> arguments);
        string Name { get;  }
    }
}