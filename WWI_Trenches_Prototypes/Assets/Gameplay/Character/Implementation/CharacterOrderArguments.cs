using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    public class CharacterOrderArguments : IOrderArguments<BasicCharacter>
    {
        public ICharacterNavigator Navigator { get; }

        public ITargetable CurrentTarget { get; }

        public Animator Animator { get; }

        public CharacterAttributesContainer Attributes { get; }

        public CharacterInventory Inventory { get; }

        public Vector3? Destination { get; }

        public CharacterOrderArguments(BasicCharacter basicCharacter)
        {
            Navigator = basicCharacter.Navigator;
            Animator = basicCharacter.Animator;
            Attributes = basicCharacter.Attributes;
            Destination = basicCharacter.Destination;
            Inventory = basicCharacter.Inventory;
            CurrentTarget = basicCharacter.CurrentTarget;
        }
    }
}