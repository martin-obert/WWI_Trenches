using System;
using Assets.IoC;
using UnityEngine;

namespace Assets.Gameplay.Abstract
{
    public abstract class Singleton<T> : MonoBehaviorDependencyResolver where T : MonoBehaviour, IDisposable
    {
        public static T Instance { get; protected set; }

        protected void CreateSingleton(T insntace, Instancing lifetime = Instancing.Singleton)
        {
            if (Instance && !insntace.Equals(Instance))
            {
                Destroy(Instance.gameObject);
            }

            Instance = insntace;

            Injection.Instance.Register(insntace);

            ResolveDependencies();
        }

        protected void GCSingleton(T instance)
        {
            if (instance == Instance)
                Dispose();
        }

        public override void Dispose()
        {
            Instance = null;

            base.Dispose();
        }
    }
}