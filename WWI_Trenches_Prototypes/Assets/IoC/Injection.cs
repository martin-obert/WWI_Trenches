using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.IoC
{
    public sealed class Injection : IDisposable
    {
        private readonly IDictionary<Type, object> _instances;
        private readonly IDictionary<Type, List<Action<object>>> _observers;

        private Injection()
        {
            _instances = new ConcurrentDictionary<Type, object>();
            _observers = new ConcurrentDictionary<Type, List<Action<object>>>();
        }


        public void Get<T>(Action<T> resolveCallback)
        {
            var type = typeof(T);
            Get(type, o => resolveCallback((T)o));
        }

        public void Get(Type type, Action<object> resolveCallback)
        {
            object result;

            if (!_instances.TryGetValue(type, out result))
            {
                AddObserver(type, resolveCallback);
            }
            else
            {
                resolveCallback(result);
            }
        }

        public void Register(Type type, object instance, bool @override = false)
        {
            Debug.Log("Registering " + type);
            bool contains;

            if ((contains = _instances.ContainsKey(type)) && !@override)
            {
                throw new ArgumentException("Instance of type " + type.FullName + " is already registered!");
            }

            if (contains)
            {
                _instances[type] = instance;
            }
            else
            {
                _instances.Add(type, instance);
            }



            List<Action<object>> observers;

            if (!_observers.TryGetValue(type, out observers)) return;



            foreach (var observer in observers)
            {
                observer(instance);
            }
        }

        public void Register<T>(T instance, bool @override = false)
        {
            var type = typeof(T);
            Register(type, instance, @override);
        }

        /// <summary>
        /// Add new observer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        private void AddObserver<T>(Type type, Action<T> callback)
        {
            List<Action<object>> actions;

            if (!_observers.TryGetValue(type, out actions))
            {
                actions = new List<Action<object>>();

                _observers.Add(type, actions);
            }
            _observers[type].Add(o => callback((T)o));
        }

        public static Injection Instance => Nested.Injection;

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly Injection Injection = new Injection();
        }

        /// <summary>
        /// Disposes all registered instances and unresolved observers
        /// </summary>
        public void Dispose()
        {
            _instances.Clear();

            if (_observers.Count > 0)
                foreach (var observer in _observers)
                {
                    observer.Value.Clear();
                }
            _observers.Clear();
        }

        public void Unregister<T>(T dep)
        {
            var type = typeof(T);
            if (_instances.ContainsKey(type))
            {
                List<Action<object>> actions;
                if (_observers.TryGetValue(type, out actions))
                {
                    foreach (var action in actions)
                    {
                        action(null);
                    }
                    //Todo: clear if singleton
                    //                    _observers[type].Clear();
                }
                _instances.Remove(type);
            }
        }
    }
}