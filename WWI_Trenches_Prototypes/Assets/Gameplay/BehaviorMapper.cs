using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gameplay
{
    public abstract class BehaviorMapper<T> : ScriptableObject
    {
        protected IReadOnlyCollection<ICharacterBehavior<T>> _behaviors;

        public abstract ICharacterBehavior<T> GetBehaviorFromState(T character);
    }
}