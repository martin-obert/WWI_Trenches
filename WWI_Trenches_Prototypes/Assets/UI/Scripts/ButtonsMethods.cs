using Assets.IoC;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public class ButtonsMethods : MonoBehaviour
    {
        private MenuManager _manager;

        private void Awake()
        {
            _manager = InjectService.Instance.GetInstance<MenuManager>(manager =>
            {
                if (!_manager)
                    _manager = manager;
            });
        }

        public void SwitchMainMenu()
        {
            _manager.CurrentMenu = MenuType.Main;
        }

        public void SwitchSettingsMenu()
        {
            _manager.CurrentMenu = MenuType.Settings;
        }
    }
}