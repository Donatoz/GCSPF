using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace Neat.ViewModels
{
    public partial class PopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message;
        [ObservableProperty]
        private string icon;
        [ObservableProperty]
        private float iconSize;
        [ObservableProperty]
        private string state;

        public Color Color { get; }
        public Brush ColorBrush { get; }

        public PopupViewModel(string message, string icon = null, float iconSize = 0, string color = "DarkGrayColor")
        {
            Message = message;
            Icon = icon;
            IconSize = iconSize;
            Color = App.Current.GetResource<Color>(color);
            ColorBrush = new SolidColorBrush(Color);
            Close();
        }
        
        public void Show()
        {
            State = "FadeIn";
        }

        public void Hide()
        {
            State = "FadeOut";
        }

        public void Close()
        {
            State = "Close";
        }
    }
}
