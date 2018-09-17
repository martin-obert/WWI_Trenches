using System;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Abstract
{
    public abstract class Singleton<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        public bool AreDependenciesResolved { get; protected set; }

        private int _dependencies = 0;

        protected void Dependency<TDependency>(Action<TDependency> function) where TDependency : UnityEngine.Object
        {
            _dependencies++;
            InjectService.Instance.Observe<TDependency>(o =>
            {
                function(o as TDependency);
                _dependencies--;
                if(_dependencies <= 0)
                    DependenciesResolved();
            });
        }

        protected virtual void DependenciesResolved()
        {
            AreDependenciesResolved = true;
        }

        protected void CreateSingleton(T insntace, Instancing lifetime = Instancing.Singleton)
        {
            if (Instance && !insntace.Equals(Instance))
            {
                Destroy(Instance.gameObject);
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