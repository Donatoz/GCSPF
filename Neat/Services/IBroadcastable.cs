using System;

namespace Neat.Services
{
    public interface IBroadcastable<T>
    {
        event Action<T> OnMessageBroadcasted;
        void Broadcast(T message);
    }
}
