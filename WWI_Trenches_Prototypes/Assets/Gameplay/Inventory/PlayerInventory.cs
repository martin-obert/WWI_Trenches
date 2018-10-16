using System;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public interface IItem
    {
        bool IsStackable { get; }
        string Name { get; }
        int MaxStack { get; }
        int Amount { get; }
    }

    public interface IWeapon : IItem
    {
        void StartFiring();

        void StopFiring();

        GameObject Target { get; set; }
        
        bool IsInRange { get; }

        bool CanFire { get; }

        float AttackSpeed { get; }

        event EventHandler<IWeapon> IsInRangeChanged;
    }

    public abstract class ItemSlot : ScriptableObject
    {
        protected IItem StoredItem { get; set; }

        public virtual bool IsOccupied => StoredItem != null && StoredItem.IsStackable && StoredItem.MaxStack < StoredItem.Amount;
    }

    public class InventorySlot : ItemSlot
    {
        public IItem Item => StoredItem;
    }

    public class DedicatedInventorySlot<T> : ItemSlot where T : IItem
    {
        public T Item
        {
            get { return (T)StoredItem; }
            set { StoredItem = value; }
        }

    }

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        private DedicatedInventorySlot<IWeapon> _mainWeapon;
        
        public IWeapon MainWeapon => _mainWeapon?.Item;

    }
}