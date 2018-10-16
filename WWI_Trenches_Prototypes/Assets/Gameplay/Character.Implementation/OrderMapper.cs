using System.Collections.Generic;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine;

namespace Assets.Gameplay.Character.Implementation
{
    public abstract class OrderMapper<T> : ScriptableObject
    {
        protected IReadOnlyCollection<ICharacterOrder<T>> Behaviors { get;  set; }

        public abstract ICharacterOrder<T> GetBehaviorFromState(T character);
    }
}