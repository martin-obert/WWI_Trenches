using System;
using Assets.Gameplay.Character;
using Assets.Gameplay.Inventory.Items;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public interface IEquipable
    {
        IIdentificable Owner { get; }

        void Equip<TOwner>(TOwner owner) where TOwner : ICharacterProxy<TOwner>;
        void Unequip<TOwner>(TOwner owner) where TOwner : ICharacterProxy<TOwner>;
    }

    
    public class CharacterEquipment 
    {
        public EquipableItemSlot<IWeapon> MainWeapon { get; }

        private ProjectilesManager _projectilesManager;

        private int? _ownerId;

        public void BindEquipment<TCharacter>(ICharacterProxy<TCharacter> character)
        {
            _ownerId = character.Id;
        }


        public CharacterEquipment()
        {
            MainWeapon = new EquipableItemSlot<IWeapon>();
            MainWeapon.ItemChanged += MainWeaponOnItemChanged;

            InjectService.Instance.GetInstance<ProjectilesManager>(instance => _projectilesManager = instance);
        }

        private void MainWeaponOnItemChanged(object sender, EquipableItemSlot<IWeapon>.InventorySlotEventArgs e)
        {
            if (!_ownerId.HasValue)
            {
                Debug.LogWarning("Equiping weapon to unbound equipment " + this);
                return;
            }

            if (e.PreviousItem != null)
            {
                _projectilesManager.UnregisterWeapon(_ownerId.Value, e.PreviousItem);
            }

            if (e.CurrentItem != null)
            {
                _projectilesManager.RegisterWeapon(_ownerId.Value, e.PreviousItem);
            }
        }



    }

    [CreateAssetMenu(fileName = "Character Inventory", menuName = "Character/Basic/Inventory")]
    public class CharacterInventory : ScriptableObject
    {
        
    }
}