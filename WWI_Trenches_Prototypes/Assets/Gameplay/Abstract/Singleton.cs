using UnityEngine;

namespace Assets.Gameplay.Abstract
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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

        protected void CGSingleton(T instance)
        {
            if (instance == Instance)
                Instance = null;
        }
    }
}