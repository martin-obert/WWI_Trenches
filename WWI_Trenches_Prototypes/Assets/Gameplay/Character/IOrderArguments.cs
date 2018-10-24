using Assets.Gameplay.Character.Implementation;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character
{
    public interface IOrderArguments<TCharacter>
    {
        ICharacterNavigator Navigator { get; }

        ITargetable CurrentTarget { get; }

        IHumanoidSkeletonProxy SkeletonProxy { get; }

        Animator Animator { get; }

        CharacterAttributesContainer Attributes { get; }

        CharacterEquipment Equipment { get; }

        Vector3? Destination { get; }
    }
}