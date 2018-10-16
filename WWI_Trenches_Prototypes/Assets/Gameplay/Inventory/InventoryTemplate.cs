using Assets.Gameplay.Inventory.Items;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public abstract  class  InventoryTemplate : ScriptableObject
    {
        public abstract Weapon MainWeapon { get; }
    }
}