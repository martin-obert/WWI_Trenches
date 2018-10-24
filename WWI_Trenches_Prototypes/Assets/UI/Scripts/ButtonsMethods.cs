﻿using Assets.Gameplay;
using Assets.IoC;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public class ButtonsMethods : MonoBehaviour
    {
        private MenuManager _manager;

        private void Awake()
        {
            InjectService.Instance.GetInstance<MenuManager>(manager =>
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

        public void Play()
        {
            _manager.CurrentMenu = MenuType.HUD;
            GameManager.Instance?.StartLevel();
        }
    }
}