using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    public class CharacterOrderArguments : IOrderArguments<BasicCharacter>
    {
        public ICharacterNavigator Navigator { get; }

        public ITargetable CurrentTarget { get; }
        public IHumanoidSkeletonProxy SkeletonProxy { get; }

        public Animator Animator { get; }

        public CharacterAttributesContainer Attributes { get; }

        public CharacterEquipment Equipment { get; }

        public Vector3? Destination { get; }

        public CharacterOrderArguments(BasicCharacter basicCharacter)
        {
            Navigator = basicCharacter.Navigator;
            Animator = basicCharacter.Animator;
            Attributes = basicCharacter.Attributes;
            Destination = basicCharacter.Destination;
            Equipment = basicCharacter.Equipment;
            CurrentTarget = basicCharacter.CurrentTarget;
            SkeletonProxy = basicCharacter.SkeletonProxy;
        }
    }
}