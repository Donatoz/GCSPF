using System.ComponentModel;

namespace Neat.Utils
{
    public class NotifiableProperty<T> : INotifyPropertyChanged
    {
        private T value;
        public T Value { get => value; set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public NotifiableProperty(ReactiveProperty<T> source)
        {
            Value = source.Value;
            source.OnChanged += val => Value = val;
        }
    }
}
