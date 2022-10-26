namespace Neat.Messages
{
    public class ViewMessage
    {
        public ViewMessageType MessageType { get; }

        public ViewMessage(ViewMessageType messageType)
        {
            MessageType = messageType;
        }
    }

    public enum ViewMessageType
    {
        PopupClose
    }
}
