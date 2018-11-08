using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.IoC
{
    public abstract class ScriptableObjectDependencyResolver : ScriptableObject, IDependencyResolver
    {
        void Awake()
        {
            OnAwakeHandle();
            ResolveDependencies();
        }

        void OnDestroy()
        {
            Injection.Instance.Unregister(GetType());
            OnDestroyHandle();
            Dispose();
        }

        /// <summary>
        /// Called after Scriptable object Awake
        /// <see cref="Awake"/>
        /// </summary>
        protected abstract void OnAwakeHandle();

        /// <summary>
        /// Called after Scriptable object OnDestroy. After this object is disposed
        /// <see cref="OnDestroy"/>
        /// <see cref="Dispose"/>
        /// </summary>
        protected abstract void OnDestroyHandle();

       
        public bool AreDependenciesResolved { get; private set; }

        private int _dependencies = 0;

        public void DependenciesResolved()
        {
            AreDependenciesResolved = true;
            OnDependeciesResolved();
        }

        protected virtual void OnDependeciesResolved() { }

        private readonly Stack<Action> _registerActions = new Stack<Action>();

        /// <summary>
        /// Add registrations to Injection Service
        /// </summary>
        public void ResolveDependencies()
        {
            foreach (var registerAction in _registerActions)
            {
                registerAction();
            }
            _registerActions.Clear();
        }

        /// <summary>
        /// Registers new dependency for this instance that has to be resolved.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <param name="function"></param>
        public void Dependency<TDependency>(Action<TDependency> function) where TDependency : UnityEngine.Object
        {
            _dependencies++;
            _registerActions.Push(() =>
            {
                Injection.Instance.Get<TDependency>(o =>
                {
                    function(o);
                    _dependencies--;
                    if (_dependencies <= 0)
                        DependenciesResolved();
                });
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// Clears unresolved dependencies.
        /// </summary>
        public virtual void Dispose()
        {
            _registerActions.Clear();
        }
    }
}