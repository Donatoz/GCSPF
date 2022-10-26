using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Neat.Messages
{
    public class PageChangedMessage : ValueChangedMessage<string>
    {
        public PageChangedMessage(string value) : base(value)
        {
        }
    }
}
