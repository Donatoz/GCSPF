using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Neat.Messages;
using Neat.Services;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        public ICommand OpenPackManager { get; }
        public ICommand OpenFlowManager { get; }

        public HomePageViewModel()
        {
            OpenPackManager = new RelayCommand(() => 
            {
                WeakReferenceMessenger.Default.Send(new PageChangedMessage(StaticPageNamespace.PackManagerDescriptor));
            });

            OpenFlowManager = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(NotificationMessage.NotImplementedMessage);
            });
        }
    }
}
