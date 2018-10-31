using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.IoC
{
    public abstract class MonoBehaviorDependencyResolver : MonoBehaviour, IDependencyResolver
    {
        private readonly Stack<Action> _registerActions = new Stack<Action>();

        private int _dependencies = 0;

        public bool AreDependenciesResolved { get; private set; }

        void Awake()
        {
            OnAwakeHandle();
        }

        void OnDestroy()
        {
            OnDestroyHandle();
            Dispose();
        }

        /// <summary>
        /// All dependencies has been resolved.
        /// </summary>
        public void DependenciesResolved()
        {
            AreDependenciesResolved = true;
            OnDependeciesResolved();
        }

        /// <summary>
        /// Called after dependencies has been resolved.
        /// <see cref="DependenciesResolved"/>
        /// </summary>
        protected virtual void OnDependeciesResolved() { }

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
        /// Add new dependency for this instance that has to be resolved.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <param name="function"></param>
        protected void Dependency<TDependency>(Action<TDependency> function) where TDependency : UnityEngine.Object
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

        /// <summary>
        /// Handle for mono behavior method Awake
        /// <see cref="Awake"/>
        /// </summary>
        protected abstract void OnAwakeHandle();

        /// <summary>
        /// Handle for mono behavior method OnDestroy
        /// <see cref="OnDestroy"/>
        /// </summary>
        protected abstract void OnDestroyHandle();
    }
}