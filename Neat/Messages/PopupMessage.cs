using Neat.ViewModels;

namespace Neat.Messages
{
    public class PopupMessage
    {
        public string Message { get; }
        public string Icon { get; }
        public float IconSize { get; }
        public string IconColor { get; }
        public int Lifetime { get; }

        public PopupMessage(string message, string icon = null, float iconSize = 0, string color = "DarkGrayColor", int lifetime = 5000)
        {
            Message = message;
            Icon = icon;
            IconSize = iconSize;
            IconColor = color;
            Lifetime = lifetime;
        }

        public PopupViewModel CreateViewModel()
        {
            return new PopupViewModel(Message, Icon, IconSize, IconColor);
        }
    }
}
