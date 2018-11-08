using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Assets.IoC
{
    public abstract class DependentJobComponentSystem : JobComponentSystem, IDisposable
    {
        private int _dependencies;
        private List<Action> Dependencies;
        protected void Depenecy<T>(Action<T> callback)
        {
            _dependencies++;
            Dependencies.Add(() =>
            {
                Injection.Instance.Get<T>(o =>
                {
                    _dependencies--;

                    callback(o);

                    if (_dependencies <= 0)
                    {
                        OnDependenciesResolved();
                    }
                });
            });
        }

        protected virtual void SetDependencies()
        {

        }

        protected override void OnCreateManager()
        {
            SetDependencies();

            foreach (var dependency in Dependencies)
            {
                dependency();
            }

            Injection.Instance.Register(this);

            AfterCreateManager();
        }

        protected override void OnDestroyManager()
        {
            Dependencies.Clear();
            if (_dependencies > 0)
            {
                DependenciesFailed();
                _dependencies = 0;
            }
            AfterDestroyManager();
        }
        protected virtual void DependenciesFailed() { }

        protected virtual void AfterDestroyManager()
        {
            
        }

        protected virtual void AfterCreateManager()
        {

        }


        protected virtual void OnDependenciesResolved()
        {

        }

        public abstract void Dispose();
    }
}