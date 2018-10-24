﻿using Assets.Gameplay.Abstract;
using Assets.Gameplay.Character.Implementation;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Factories
{
    public class CharacterFactory : Singleton<CharacterFactory>
    {
        private Bootstrapper _bootstrapper;

        void OnEnable()
        {
            CreateSingleton(this);
            InjectService.Instance.GetInstance<Bootstrapper>(x => _bootstrapper = x);
        }

        void OnDestroy()
        {
            GCSingleton(this);
        }

        public BasicCharacter CreatePlayerInstance()
        {
            var instance = Instantiate(_bootstrapper.PlayerPrefab);

            var itemInstance = Instantiate(_bootstrapper.PlayerMainWeapon);

            //Todo: equipment provider
            instance.Equipment.MainWeapon.SetItem(itemInstance, instance);


            return instance;
        }
    }
}