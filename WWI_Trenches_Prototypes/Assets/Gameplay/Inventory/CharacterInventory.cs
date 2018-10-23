using Assets.Gameplay.Character;
using Assets.Gameplay.Inventory.Items;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "Character Inventory", menuName = "Character/Basic/Inventory")]
    public class CharacterInventory : ScriptableObject
    {
        [SerializeField]
        private InventoryTemplate _template;


        private ProjectilesManager _projectilesManager;

        void Awake()
        {
            _projectilesManager = InjectService.Instance.GetInstance<ProjectilesManager>(instance => _projectilesManager = instance);

            //Todo: implement this
            //if (_template)
            //{
            //    var instance = Instantiate(_template.MainWeapon, _mainWeaponSpot);
            //    instance.transform.localPosition = Vector3.zero;
            //    EquipMainWeapon(instance);
            //}
        }

        
        private readonly DedicatedInventorySlot<IWeapon> _mainWeapon = new DedicatedInventorySlot<IWeapon>();

        public IWeapon MainWeapon => _mainWeapon?.Item;

        private int? _ownerId;

        public void BindInventory<TCharacter>(ICharacterProxy<TCharacter> character)
        {
            _ownerId = character.Id;
        }

        public void EquipMainWeapon(IWeapon weapon)
        {
            if (!_ownerId.HasValue)
            {
                Debug.LogError("This inventory has not yet been bound");
                return;
            }

            if (_mainWeapon.IsOccupied)
            {
                _projectilesManager.UnregisterWeapon(_ownerId.Value, _mainWeapon.Item);
            }


            _projectilesManager.RegisterWeapon(_ownerId.Value, weapon);

            _mainWeapon.Item = weapon;
        }

        public void UnequipMainWeapon()
        {
            if (!_mainWeapon.IsOccupied || !_ownerId.HasValue)
            {
                return;
            }

            _projectilesManager.UnregisterWeapon(_ownerId.Value, MainWeapon);

            _mainWeapon.Item = null;
        }
    }
}