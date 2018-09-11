using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.IoC
{
    public class InjectContainer
    {
        public readonly IDictionary<Type, object> Singletons = new Dictionary<Type, object>();

        public void Register<T>(object instance)
        {
            var type = typeof(T);
            if (Singletons.ContainsKey(type))
            {
                Debug.LogWarning("This type is already registered: " + type);
                return;
            }

            Singletons[type] = instance;
        }

        public void UnRegister<T>()
        {
            Singletons.Remove(typeof(T));
        }
    }
}