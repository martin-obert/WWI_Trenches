using Assets.Gameplay.Character.Implementation;
using UnityEngine.Events;

namespace Assets.Gameplay.Character.Interfaces
{
    public class ObservableAttribute<T> : BasicAttribute<T>
    {
        private object _currentValue;

        protected override T Value(T value = default(T))
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
}