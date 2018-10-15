using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Gameplay
{
    public interface ICharacterNavigator<T>
    {
        void Teleport(Vector3 position);
        void Move(Vector3 position);
        void Stop();
        void Disable();
        void Enable();
    }

    public interface ICharacterAttribute
    {
        object MinValue { get; }
        object CurrentValue { get; set; }
        object MaxValue { get; }
        string DisplayName { get; }
        int Hash { get; }
    }

    public class BasicAttribute<T> : ICharacterAttribute
    {
        private object _currentValue;

        public BasicAttribute(string name, string displayName, T currentValue, T minValue, T maxVaue)
        {
            MinValue = minValue;
            MaxValue = maxVaue;
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

        public virtual T Value(T value = default(T))
        {
            _currentValue = value;
            return (T)_currentValue;
        }
    }

    public class BasicAttributeValueChanged<T> : UnityEvent<T>
    {
        
    }

    public class ObservableAttribute<T> : BasicAttribute<T>
    {
        private object _currentValue;

        public override T Value(T value = default(T))
        {
            var val = base.Value(value);

            Invoke(val);

            return val;
        }

        private readonly UnityEvent<T> _valueChanged;

        private void Invoke(T value)
        {
            _valueChanged?.Invoke(value);
        }

        public void Subscribe(UnityAction<T> action)
        {
            _valueChanged.AddListener(action);
        }

        public ObservableAttribute(string name, string displayName, T currentValue, T minValue, T maxVaue) : base(name, displayName, currentValue, minValue, maxVaue)
        {
            _valueChanged = new BasicAttributeValueChanged<T>();
        }
    }

    public class AttributesContainer
    {
        private readonly List<ICharacterAttribute> _attributes;

        public AttributesContainer(List<ICharacterAttribute> attributes)
        {
            _attributes = attributes;
            if (_attributes == null)
            {
                _attributes = new List<ICharacterAttribute>();
            }
        }

        public IReadOnlyCollection<ICharacterAttribute> Attributes => _attributes;
    }

    public class BasicCharacterAttributesContainer
    {
        public ObservableAttribute<float> Speed { get; }

        public BasicCharacterAttributesContainer()
        {
            Speed = new ObservableAttribute<float>("speed", "Speed", 3, 0, 5);
        }
    }
}