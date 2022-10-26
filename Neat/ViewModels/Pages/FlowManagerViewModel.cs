using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Neat.Messages;
using Neat.Services;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class FlowManagerViewModel : ObservableObject
    {
        public ICommand GoBack { get; }

        public FlowManagerViewModel()
        {
            GoBack = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(new PageChangedMessage(StaticPageNamespace.HomePageDescriptor));
            });
        }
    }
}
