using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Player.Orders
{
    public class PlayerOrderArguments : IOrderArguments<PlayerController>
    {
        public ICharacterNavigator<PlayerController> Navigator { get;  }
        public ITargetable CurrentTarget { get; }

        public Vector3? Destination { get; }

        public Animator Animator { get; }

        public CharacterAttributesContainer Attributes { get;  }

        public CharacterInventory Inventory { get; }


        public PlayerOrderArguments(Animator animator, Vector3? destination, ITargetable currentTarget, ICharacterNavigator<PlayerController> navigator, CharacterAttributesContainer attributes, CharacterInventory inventory)
        {
            Animator = animator;
            Destination = destination;
            CurrentTarget = currentTarget;
            Navigator = navigator;
            Attributes = attributes;
            Inventory = inventory;
        }
    }
}