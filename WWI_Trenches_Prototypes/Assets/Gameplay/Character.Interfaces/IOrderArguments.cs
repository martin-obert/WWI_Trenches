using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Interfaces
{
    public interface IOrderArguments<TCharacter>
    {
        ICharacterNavigator<TCharacter> Navigator { get; }

        ITargetable CurrentTarget { get; }

        Animator Animator { get; }

        CharacterAttributesContainer Attributes { get; }

        CharacterInventory Inventory { get; }

        Vector3? Destination { get; }
    }
}