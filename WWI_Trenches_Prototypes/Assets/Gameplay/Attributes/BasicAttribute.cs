using System;
using Assets.Gameplay.Character;

namespace Assets.Gameplay.Attributes
{
    public class BasicAttribute<T> : ICharacterAttribute where T : IComparable<T>
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



        public object MaxValue { get; }

        public string DisplayName { get; }

        public string Name { get; }

        public int Hash { get; }

        public T Value()
        {
            return (T) CurrentValue;
        }

        public virtual T Value(T value)
        {
            if (value != null && value.CompareTo((T)CurrentValue) != 0)
                _currentValue = Clamp != null ? Clamp((T)MinValue, value, (T)MaxValue) : value;

            return (T)_currentValue;
        }

    }
}