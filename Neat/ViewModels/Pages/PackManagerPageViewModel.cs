using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Neat.Cloud;
using Neat.Messages;
using Neat.Models;
using Neat.Services;
using Neat.Utils;
using Neat.ViewModels.Packages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class PackManagerPageViewModel : ObservableObject, IPackageHost
    {
        public MainPageViewModel Parent { get; set; }

        public ObservableCollection<PackageViewModel> LoadedPackages { get; }
        public ObservableCollection<PackageReferenceViewModel> AvailablePackages { get; }

        public VaryingServiceViewModel CloudServiceStatus { get; }

        [ObservableProperty]
        private int packagesCount;
        [ObservableProperty]
        private int selectedPackage;

        public ICommand AddPackage { get; }
        public ICommand OpenPackage { get; }
        public ICommand FetchPackage { get; }
        public ICommand SavePackage { get; }
        public ICommand TabChanged { get; }
        public ICommand SaveAllPackages { get; }
        public ICommand CloseAllPackages { get; }
        public ICommand GoBack { get; }

        public ICommand ReconnectCloud { get; }
        public ICommand OpenApi { get; }

        private readonly CloudPackageFetchMessageHandler cloudPackageHandler;

        public PackManagerPageViewModel()
        {
            AvailablePackages = new ObservableCollection<PackageReferenceViewModel>();
            FetchAvailablePackages();

            LoadedPackages = new ObservableCollection<PackageViewModel>();
            AddPackage = new RelayCommand(OnAddPackage);
            TabChanged = new RelayCommand<SelectionChangedEventArgs>(OnTabChanged);
            OpenPackage = new RelayCommand(OnOpenPackage);
            FetchPackage = new RelayCommand(OnFetchPackage);
            SavePackage = new RelayCommand(OnSavePackage);
            ReconnectCloud = new RelayCommand(OnCloudReconnect);
            OpenApi = new RelayCommand(OpenExternalApi);
            SaveAllPackages = new RelayCommand(OnSaveAllPackages);
            CloseAllPackages = new RelayCommand(OnCloseAllPakcages);

            GoBack = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(new PageChangedMessage(StaticPageNamespace.HomePageDescriptor));
            });

            LoadedPackages.CollectionChanged += delegate
            {
                PackagesCount = LoadedPackages.Count;
            };
            cloudPackageHandler = new CloudPackageFetchMessageHandler(this);

            CloudServiceStatus = new VaryingServiceViewModel(App.Current.Services.GetService<ICloudService>());

            WeakReferenceMessenger.Default.Register<PackageClosedMessage>(this, OnPackageClosed);

            App.Current.Services.GetService<IDialogResultProcessor>().AddHandler(new DialogResultHandler(typeof(CreatePackageDialogResult), OnCreatePackageDialogResult));
        }

        private void FetchAvailablePackages()
        {
            var packagesPath = App.Current.Services.GetService<IPathProvider>().PackagesPath;

            var files = Directory.GetFiles(packagesPath);

            foreach (var f in files.Where(f => f.EndsWith(".json")))
            {
                var bytes = File.ReadAllBytes(f);
                var reference = JsonSerializer.Deserialize<PackageReferenceViewModel>(bytes);

                reference.OnOpen += delegate
                {
                    var package = GetPackage(f);

                    if (package != null)
                    {
                        IncludePackage(package);
                    }
                };

                AvailablePackages.Add(reference);
            }
        }

        private void OnSaveAllPackages()
        {
            var saved = 0;

            foreach (var p in LoadedPackages)
            {
                var s = p.Model.Save();
                if (s) saved++;
            }

            WeakReferenceMessenger.Default.Send(new PopupMessage(
                saved == LoadedPackages.Count ? $"All packages were successfully saved" : $"{LoadedPackages.Count - saved} packages failed to save",
                saved == LoadedPackages.Count ? "Check" : "Info",
                saved == LoadedPackages.Count ? 20 : 10,
                saved == LoadedPackages.Count ? "GreyGreenColor" : "DarkGrayColor"));
        }

        private void OnCloseAllPakcages()
        {
            Parent.PopupsEnabled = false;
            for (var i = LoadedPackages.Count - 1; i >= 0; i--)
            {
                ClosePackage(i);
            }
            Parent.PopupsEnabled = true;
        }

        private void OnFetchPackage()
        {
            WeakReferenceMessenger.Default.Send(new FetchPackageDialogMessage());
        }

        public void OpenExternalApi()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://console.cloud.google.com/storage/browser?project=neatapi&prefix=",
                UseShellExecute = true
            });
        }

        private async void OnCloudReconnect()
        {
            var service = App.Current.Services.GetService<ICloudService>();
            service.Disconnect();

            var e = await service.Connect();
            if (e != null)
            {
                WeakReferenceMessenger.Default.Send(NotificationMessage.Error(e));
            }
        }

        private void OnPackageClosed(object sender, PackageClosedMessage message)
        {
            var vm = LoadedPackages.FirstOrDefault(p => p.Model == message.Sender);
            if (vm != null)
            {
                ClosePackage(LoadedPackages.IndexOf(vm));
            }
        }

        public void ClosePackage(int idx)
        {
            var vm = LoadedPackages[idx];
            LoadedPackages.RemoveAt(idx);
            vm.Close();
        }

        private void OnCreatePackageDialogResult(DialogInputResult result)
        {
            var c = result as CreatePackageDialogResult;
            IncludePackage(c.Package);
            c.Package.Save();
        }

        private void OnOpenPackage()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON documents (.json)|*.json";

            var res = dialog.ShowDialog();

            if (res == true)
            {
                var file = dialog.FileName;
                var package = App.Current.Services.GetService<IPackageProvider>().FetchPackage(file);

                if (package != null)
                {
                    IncludePackage(package);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new PopupMessage("Failed to open selected package.", "Close", 20, "GreyRedColor"));
                }
            }
        }

        private void OnSavePackage()
        {
            if (LoadedPackages.Count <= SelectedPackage) return;

            var currentPackage = LoadedPackages[SelectedPackage].Model;
            var saved = currentPackage.Save();

            WeakReferenceMessenger.Default.Send(new PopupMessage(
                saved ? $"{currentPackage.Name} was successfully saved." : "An error appeared during package save.",
                saved ? "Check" : "Close",
                20,
                saved ? "GreyGreenColor" : "GreyRedColor"));
        }
        
        public void IncludePackage(Package package)
        {
            var existingPackage = LoadedPackages.FirstOrDefault(p => p.Model.Id == package.Id);

            if (existingPackage != null)
            {
                SelectPackage(LoadedPackages.IndexOf(existingPackage));
                return;
            }

            LoadedPackages.Add(new PackageViewModel(package, this));
            SelectPackage(LoadedPackages.Count - 1);
        }

        public void SelectPackage(int idx)
        {
            SelectedPackage = idx;
            LoadedPackages[SelectedPackage].Selected = true;
        }

        private void OnAddPackage()
        {
            WeakReferenceMessenger.Default.Send(new CreatePackageDialogMessage());
        }

        private void OnTabChanged(SelectionChangedEventArgs? e)
        {
            if (e != null)
            {
                var removed = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as PackageViewModel : null;
                var added = e.AddedItems.Count > 0 ? e.AddedItems[0] as PackageViewModel : null;

                if (removed != null)
                {
                    removed.Selected = false;
                }

                if (added != null)
                {
                    added.Selected = true;
                }
            }
        }

        private Package? GetPackage(string path)
        {
            return App.Current.Services.GetService<IPackageProvider>()?.FetchPackage(path);
        }
    }
}
