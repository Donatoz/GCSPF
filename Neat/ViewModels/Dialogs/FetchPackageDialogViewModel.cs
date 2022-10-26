using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Neat.Extensions;
using Neat.Messages;
using Neat.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class FetchPackageDialogViewModel : DialogViewModel
    {
        [ObservableProperty]
        private CloudPackageViewModel selectedPackage;
        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private string root;

        public VaryingServiceViewModel ServiceStatus { get; }
        public NotifiableProperty<bool> IsServiceActive { get; }
        public ObservableCollection<CloudPackageViewModel> Packages { get; }

        public ICommand Reconnect { get; }
        public ICommand GetAvailable { get; }

        public FetchPackageDialogViewModel()
        {
            ServiceStatus = new VaryingServiceViewModel(App.Current.GetCloudService());
            IsServiceActive = App.Current.GetCloudService().IsAvailable.ToNotifiable();
            Packages = new ObservableCollection<CloudPackageViewModel>();
            Root = "mainhub";

            Reconnect = new RelayCommand(OnReconnect);
            GetAvailable = new RelayCommand(FetchAvailablePackages);
        }

        private async void OnReconnect()
        {
            var antipMessage = new AnticipantMessage("Trying to reconnect...");
            WeakReferenceMessenger.Default.Send(antipMessage);

            var e = await App.Current.GetCloudService().Connect();

            antipMessage.End();

            if (e != null)
            {
                WeakReferenceMessenger.Default.Send(NotificationMessage.Error(e));
            }
        }

        private async void FetchAvailablePackages()
        {
            if (Root.IsNullOrEmpty())
            {
                WeakReferenceMessenger.Default.Send(new PopupMessage("Root is required", "Close", 20, "GreyRedColor"));
                return;
            }

            var service = App.Current.GetCloudService();

            IsLoading = true;

            var list = await service.List(Root, "Packages/", 100);

            foreach (var p in list)
            {
                var spliced = p.Split('/');
                if (spliced.Length == 2)
                {
                    var vm = new CloudPackageViewModel(spliced[0]);
                    vm.OnSelected += delegate { OnPackageSelected(vm); };
                    Packages.Add(vm);
                }
            }

            IsLoading = false;
        }

        private void OnPackageSelected(CloudPackageViewModel package)
        {
            if (SelectedPackage != null)
            {
                SelectedPackage.IsSelected = false;
            }

            SelectedPackage = package;
            package.IsSelected = true;
        }

        protected override DialogInputResult CreateResult(ExitState state)
        {
            return new FetchPackageDialogResult(selectedPackage.PackageName, Root);
        }
    }

    public partial class CloudPackageViewModel : ObservableObject
    {
        public string PackageName { get; }

        [ObservableProperty]
        private bool isSelected;

        public ICommand Select { get; }

        public event Action OnSelected;

        public CloudPackageViewModel(string packageName)
        {
            PackageName = packageName;
            Select = new RelayCommand(HandleSelect);
        }

        private void HandleSelect()
        {
            OnSelected?.Invoke();
        }
    }

    public class FetchPackageDialogResult : DialogInputResult
    {
        public string PackageName { get; }
        public string Root { get; }

        public FetchPackageDialogResult(string packageName, string root, string? error = null) : base(!packageName.IsNullOrEmpty() && !root.IsNullOrEmpty(), error)
        {
            PackageName = packageName;
            Root = root;
        }
    }
}
