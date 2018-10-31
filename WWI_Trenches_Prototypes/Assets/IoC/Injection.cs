﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.IoC
{
    public sealed class Injection
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
            object result;
            if (!_instances.TryGetValue(typeof(T), out result))
            {
                AddObserver(resolveCallback);
            }
            else
            {
                resolveCallback((T)result);
            }
        }

        public void Get(Type type, Action<object> resolveCallback)
        {
            object result;
            if (!_instances.TryGetValue(type, out result))
            {
                AddObserver(resolveCallback);
            }
            else
            {
                resolveCallback(result);
            }
        }

        public void Register<T>(T instance, bool @override = false)
        {
            var type = typeof(T);

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
        }

        private void AddObserver<T>(Action<T> callback)
        {
            var type = typeof(T);

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
    }
}