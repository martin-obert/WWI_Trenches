using System;
using Assets.Gameplay.Character.Implementation;

namespace Assets.Gameplay.Attributes
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

        private event EventHandler<T> _valueChanged;

        private void Invoke(T value)
        {
            _valueChanged?.Invoke(this, value);
        }

        public void Subscribe(EventHandler<T> action)
        {
            _valueChanged += action;
        }
        public void Unsubscribe(EventHandler<T> action)
        {
            _valueChanged -= action;
        }

        public ObservableAttribute(string name, string displayName, T minValue, T currentValue, T maxVaue) : base(name, displayName,  minValue, currentValue, maxVaue)
        {
        }
    }
}