﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.IoC
{
    public enum Instancing
    {
        None,
        Singleton
    }

    internal class InjectService : ScriptableObject
    {
        private static InjectService _instance;

        public static InjectService Instance => _instance ?? (_instance = CreateInstance<InjectService>());

        private InjectContainer Container { get; } = new InjectContainer();

        private IDictionary<Type, List<Action<UnityEngine.Object>>> _callbackDictionary = new Dictionary<Type, List<Action<Object>>>();

        public void Observe<T>(Action<Object> callback) where T : Object
        {


            object current;
            if (Container.Singletons.TryGetValue(typeof(T), out current))
            {
                callback(current as T);
            }
            else
            {
                List<Action<Object>> list;

                if (!_callbackDictionary.TryGetValue(typeof(T), out list))
                {
                    list = new List<Action<Object>>();

                    _callbackDictionary.Add(typeof(T), list);
                }

                list.Add(callback);
            }
        }

        public void Register<T>(T instance, Instancing instancing = Instancing.Singleton) where T : UnityEngine.Object
        {
            if (instancing == Instancing.Singleton && Container.Singletons.ContainsKey(typeof(T)))
                return;

            if (instance)
                Container.Register<T>(instance);

            List<Action<Object>> list;
            if (_callbackDictionary.TryGetValue(typeof(T), out list))
            {
                foreach (var action in list)
                {
                    action(instance);
                }
                if (instancing == Instancing.Singleton)
                    _callbackDictionary.Remove(typeof(T));
            }
        }

        public void UnRegister<T>() where T : UnityEngine.Object
        {
            Container.UnRegister<T>();
        }

        public void GetInstance<T>(Action<T> callback) where T : UnityEngine.Object
        {
            if (Container == null)
            {
                throw new NullReferenceException("No Container has been set");
            }

            object result;

            if (Container.Singletons.TryGetValue(typeof(T), out result))
                callback(result as T);

            if (callback != null)
                Observe<T>(o => callback(o as T));
        }

        void Awake()
        {
            if (_instance && _instance != this)
            {
                DestroyImmediate(_instance);
            }

            _instance = this;
        }


        void OnDestroy()
        {
            _callbackDictionary.Clear();
            _callbackDictionary = null;
            _instance = null;
        }
    }
}