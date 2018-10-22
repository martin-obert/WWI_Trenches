using System;

namespace Assets.Gameplay.Attributes
{
    public class ObservableAttribute<T> : BasicAttribute<T> where T: IComparable<T>
    {

        public override T Value(T value)
        {
            var doInvoke = value.CompareTo((T)CurrentValue) != 0;

            var val = base.Value(value);

            if (doInvoke)
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

        public ObservableAttribute(string name, string displayName, T minValue, T currentValue, T maxVaue, Func<T, T, T, T> clamp = null) : base(name, displayName, minValue, currentValue, maxVaue, clamp)
        {
        }
    }
}