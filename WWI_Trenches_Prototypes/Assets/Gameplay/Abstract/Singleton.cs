using System;
using UnityEngine;

namespace Assets.Gameplay.Abstract
{
    public abstract class Singleton<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected void CreateSingleton(T insntace)
        {
            if (Instance && !insntace.Equals(Instance))
            {
                Destroy(Instance);
            }

            Instance = insntace;
        }

        protected void GCSingleton(T instance)
        {
            if (instance == Instance)
                Dispose();
        }

        public void Dispose()
        {

            Instance = null;
        }
    }
}