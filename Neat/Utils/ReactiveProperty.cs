using System;

namespace Neat.Utils
{
    public class ReactiveProperty<T>
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnChanged?.Invoke(this.value);
            }
        }

        public event Action<T> OnChanged;

        public ReactiveProperty(T value)
        {
            Value = value;
        }
    }
}
