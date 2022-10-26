using CommunityToolkit.Mvvm.Messaging;

namespace Neat.Messages
{
    public abstract class AppMessage
    {
        public void Send()
        {
            WeakReferenceMessenger.Default.Send(this);
        }
    }
}
