using System;

namespace Assets.IoC
{
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// Called when all dependencies has been resolved.
        /// </summary>
        void DependenciesResolved();

        /// <summary>
        /// Signals that dependencies has been resolved.
        /// </summary>
        bool AreDependenciesResolved { get; }

        ///// <summary>
        ///// Adds dependency that will be resolved for this instance.
        ///// </summary>
        ///// <typeparam name="TDependency"></typeparam>
        ///// <param name="function"></param>
        //void Dependency<TDependency>(Action<TDependency> function) where TDependency : UnityEngine.Object;
    }
}