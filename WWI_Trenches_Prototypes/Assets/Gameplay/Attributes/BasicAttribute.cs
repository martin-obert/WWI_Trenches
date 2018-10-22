using System;
using Assets.Gameplay.Character;

namespace Assets.Gameplay.Attributes
{
    public class BasicAttribute<T> : ICharacterAttribute
    {
        private object _currentValue;

        protected Func<T, T, T, T> Clamp { get; }

        public BasicAttribute(string name, string displayName, T minValue, T currentValue, T maxValue, Func<T, T, T, T> clamp = null)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            MinValue = minValue;
            CurrentValue = currentValue;
            Name = name;
            Hash = name.GetHashCode();
            DisplayName = displayName;
            Clamp = clamp;
        }

        public object MinValue { get; }

        public object CurrentValue
        {
            get { return Value(); }
            set { Value((T)value); }
        }

        public T CurrentValueTyped()
        {
            return (T)_currentValue;
        }

        public object MaxValue { get; }

        public string DisplayName { get; }

        public string Name { get; }

        public int Hash { get; }

        protected virtual T Value(T value = default(T))
        {

            _currentValue = Clamp != null ? Clamp((T)MinValue, value, (T)MaxValue) : value;

            return (T)_currentValue;
        }

    }
}