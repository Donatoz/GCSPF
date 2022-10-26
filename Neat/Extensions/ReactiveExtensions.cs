using Neat.Utils;

namespace Neat.Extensions
{
    public static class ReactiveExtensions
    {
        public static NotifiableProperty<T> ToNotifiable<T>(this ReactiveProperty<T> prop)
        {
            return new NotifiableProperty<T>(prop);
        }
    }
}
