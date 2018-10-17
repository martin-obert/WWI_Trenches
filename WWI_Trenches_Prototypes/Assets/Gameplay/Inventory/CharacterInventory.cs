using Assets.Gameplay.Inventory.Items;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Inventory
{
    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField]
        private InventoryTemplate _template;

        [SerializeField] private Transform _mainWeaponSpot;

        private ProjectilesManager _projectilesManager;

        void Start()
        {
            _projectilesManager = InjectService.Instance.GetInstance<ProjectilesManager>(instance => _projectilesManager = instance);
            if (_template)
            {
                var instance = Instantiate(_template.MainWeapon, _mainWeaponSpot);
                instance.transform.localPosition = Vector3.zero;
                EquipMainWeapon(instance);
            }

            
        }

        [SerializeField]
        private DedicatedInventorySlot<IWeapon> _mainWeapon = new DedicatedInventorySlot<IWeapon>();

        public IWeapon MainWeapon => _mainWeapon?.Item;

        public void EquipMainWeapon(IWeapon weapon)
        {
            if (_mainWeapon.IsOccupied)
            {
                _projectilesManager.UnregisterWeapon(gameObject.GetInstanceID(), _mainWeapon.Item);
            }
            print(weapon.Id);
            _projectilesManager.RegisterWeapon(gameObject.GetInstanceID(), weapon);

            _mainWeapon.Item = weapon;
        }
    }
}