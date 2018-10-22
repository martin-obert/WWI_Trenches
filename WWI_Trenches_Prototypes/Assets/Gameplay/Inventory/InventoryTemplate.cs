using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public abstract  class  InventoryTemplate : ScriptableObject
    {
        public abstract RangedWeapon MainWeapon { get; }
        public abstract MeleeWeapon MeleeWeapon { get; }
    }
}