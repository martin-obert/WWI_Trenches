using System.Collections;
using System.Linq;
using Assets.Gameplay.Abstract;
using UnityEditor;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public enum MenuType
    {
        Main,
        Settings,
        HUD
    }

    public class MenuManager : Singleton<MenuManager>
    {
        [SerializeField] private MenuController[] _menuControllers;

        [SerializeField] private float _delay = 1;

        private MenuType _currentMenu;

        public MenuController CurrentMenuController => _menuControllers.FirstOrDefault(x => x.MenuType == _currentMenu);

        public MenuType CurrentMenu
        {
            get { return _currentMenu; }
            set
            {
                SwitchMenuController(value);

                _currentMenu = value;
            }
        }

        protected override void OnEnableHandle()
        {
            CreateSingleton(this);
            foreach (var menuController in _menuControllers)
            {
                menuController.HideMenu();
            }

            StartCoroutine(DelayedSwitch(CurrentMenu));
        }

        protected override void OnDestroyHandle()
        {
            GCSingleton(this);
        }

        private IEnumerator DelayedSwitch(MenuType value)
        {
            yield return new WaitForSecondsRealtime(_delay);
            SwitchMenuController(value);
        }

        private void SwitchMenuController(MenuType value)
        {
            var currentController = CurrentMenuController;
            if (currentController)
            {
                currentController.HideMenu();
            }

            var newMenu = _menuControllers.FirstOrDefault(x => x.MenuType == value);
            if (newMenu)
            {
                newMenu.ShowMenu();
            }
        }


    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MenuManager))]
    public class MenuManagerEditor : Editor
    {
        private MenuType _lastVal;

        public override void OnInspectorGUI()
        {

            var temp = (MenuType)EditorGUILayout.EnumPopup("Current menu:", _lastVal);

            if (temp != _lastVal)
            {
                _lastVal = temp;
                ((MenuManager)target).CurrentMenu = _lastVal;


            }

            base.OnInspectorGUI();
        }
    }

#endif
}
