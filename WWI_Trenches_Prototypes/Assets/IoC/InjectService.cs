using System;
using UnityEngine;

namespace Assets.IoC
{
    public class InjectService : ScriptableObject
    {
        private static InjectService _instance;

        public static InjectService Instance => _instance ?? (_instance = CreateInstance<InjectService>());

        private InjectContainer Container { get; } = new InjectContainer();

        public void Observe<T>(Action<T>) where T : UnityEngine.Object
        {

        }

        public void Register<T>(T instance) where T : UnityEngine.Object
        {
            if (instance)
                Container.Register<T>(instance);
        }

        public void UnRegister<T>() where T : UnityEngine.Object
        {
            Container.UnRegister<T>();
        }

        public T GetInstance<T>() where T : UnityEngine.Object
        {
            if (Container == null)
            {
                throw new NullReferenceException("No Container has been set");
            }

            InstanceLifetime result;
            if (!Container.InstanceLifetimes.TryGetValue(typeof(T), out result))
            {
                return default(T);
            }

            if (result == InstanceLifetime.PerRequest)
                return FindObjectOfType<T>();

            if (result == InstanceLifetime.Singleton)
                return Container.Singletons[typeof(T)]() as T;

            return default(T);
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
            _instance = null;
        }
    }
}