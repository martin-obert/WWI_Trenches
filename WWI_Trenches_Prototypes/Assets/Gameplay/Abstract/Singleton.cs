using System;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Abstract
{
    public abstract class Singleton<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected void Dependency<TDependency>(Action<TDependency> function) where TDependency : UnityEngine.Object
        {
            InjectService.Instance.Observe<TDependency>(o => function(o as TDependency));
        }

        protected void CreateSingleton(T insntace)
        {
            if (Instance && !insntace.Equals(Instance))
            {
                Destroy(Instance);
            }

            Instance = insntace;

            InjectService.Instance.Register(insntace);
        }

        protected void GCSingleton(T instance)
        {
            if (instance == Instance)
                Dispose();
        }

        public void Dispose()
        {
            InjectService.Instance.UnRegister<T>();
            Instance = null;
        }
    }
}