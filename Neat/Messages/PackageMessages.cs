using Neat.Models;

namespace Neat.Messages
{
    public abstract class PackageMessage
    {
        public Package Sender { get; }

        protected PackageMessage(Package sender)
        {
            Sender = sender;
        }
    }

    public class PackageClosedMessage : PackageMessage
    {
        public PackageClosedMessage(Package sender) : base(sender)
        {
        }
    }
}
