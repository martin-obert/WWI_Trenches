using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface IOrderArguments<TCharacter>
    {
        ICharacterNavigator Navigator { get; }

        ITargetable CurrentTarget { get; }

        Animator Animator { get; }

        CharacterAttributesContainer Attributes { get; }

        CharacterInventory Inventory { get; }

        Vector3? Destination { get; }
    }
}