using System;

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

        private event EventHandler<T> ValueChanged;

        private void Invoke(T value)
        {
            ValueChanged?.Invoke(this, value);
        }

        public void Subscribe(EventHandler<T> action)
        {
            ValueChanged += action;
        }
        public void Unsubscribe(EventHandler<T> action)
        {
            ValueChanged -= action;
        }

        public ObservableAttribute(string name, string displayName, T minValue, T currentValue, T maxVaue, Func<T,T,T,T> clamp = null) : base(name, displayName,  minValue, currentValue, maxVaue, clamp)
        {
        }
    }
}