using System;
using Assets.IoC;

namespace Assets.SpCrsVrPrototypes.Patterns
{
    public abstract class SingletonDisposable<TSingleton> : IDisposable where TSingleton : class, new()
    {
        protected SingletonDisposable()
        {
            if (Nested.Instance != null)
                throw new ArgumentException("Creating already existing singleton " + GetType());

            Nested.Instance = this;
        }



        #region Singleton Pattern
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static SingletonDisposable<TSingleton> Instance;
        }
        #endregion Singleton Pattern

        protected void Dependency<TInstance>(Action<TInstance> callback)
        {
            Injection.Instance.Get(typeof(TInstance), o => callback((TInstance)o));
        }

        public abstract void Dispose();

    }
}