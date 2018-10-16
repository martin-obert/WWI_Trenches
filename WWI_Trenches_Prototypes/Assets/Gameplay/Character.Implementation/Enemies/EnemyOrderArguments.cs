using Assets.Gameplay.Character.Interfaces;
using Assets.Gameplay.Inventory;

namespace Assets.Gameplay.Character.Implementation.Enemies
{
    public class EnemyOrderArguments : IOrderArguments<GruntController>
    {
        public EnemyOrderArguments(ITargetable target, CharacterInventory inventory, ICharacterNavigator<GruntController> navigator)
        {
            Target = target;
            Inventory = inventory;
            Navigator = navigator;
        }

        public ICharacterNavigator<GruntController> Navigator { get; }

        public ITargetable Target { get; }

        public CharacterInventory Inventory { get; }
    }
}