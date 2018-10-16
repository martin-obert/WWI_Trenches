using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Character.Implementation
{
    public class BasicAttribute<T> : ICharacterAttribute
    {
        private object _currentValue;

        public BasicAttribute(string name, string displayName, T currentValue, T minValue, T maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            MinValue = minValue;
            CurrentValue = currentValue;
            Name = name;
            Hash = name.GetHashCode();
            DisplayName = displayName;
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

        protected virtual T Value(T value = default(T))
        {
            _currentValue = value;
            return (T)_currentValue;
        }
    }
}