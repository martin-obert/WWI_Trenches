using Assets.Gameplay.Character.Implementation.Attributes;
using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyOrderArguments : IOrderArguments<GruntController>
    {
        public EnemyOrderArguments(ITargetable target, CharacterInventory inventory, ICharacterNavigator<GruntController> navigator)
        {
            CurrentTarget = target;
            Inventory = inventory;
            Navigator = navigator;
        }

        public ICharacterNavigator<GruntController> Navigator { get; }
        public ITargetable CurrentTarget { get; }
        public Animator Animator { get; }
        public BasicCharacterAttributesContainer Attributes { get; }

        public CharacterInventory Inventory { get; }
        public Vector3 Destination { get; }
    }
}