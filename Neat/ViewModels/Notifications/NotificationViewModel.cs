using CommunityToolkit.Mvvm.ComponentModel;
using Neat.Messages;
using System;
using System.Windows.Media;

namespace Neat.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message;
        [ObservableProperty]
        private string title;
        
        public Brush Color { get; }
        public NotificationOption[] Options { get; } 

        public NotificationViewModel(string message, string title, string color, params NotificationOption[] options)
        {
            Message = message;
            Options = options ?? Array.Empty<NotificationOption>();
            Title = title;
            Color = new SolidColorBrush(App.Current.GetResource<Color>(color));
        }
    }
}
