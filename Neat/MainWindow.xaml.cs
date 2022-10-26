using CommunityToolkit.Mvvm.ComponentModel;
using Neat.Services;
using Neat.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Neat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IPageProvider
    {
        public MainPageViewModel ViewModel { get; set; }

        public HomePageViewModel HomePage { get; private set; }
        public PackManagerPageViewModel PackManagerPage { get; private set; }
        public FlowManagerViewModel FlowManagerPage { get; private set; }

        private readonly Dictionary<string, ObservableObject> pagesMap;

        public MainWindow()
        {
            HomePage = new HomePageViewModel();
            PackManagerPage = new PackManagerPageViewModel();
            FlowManagerPage = new FlowManagerViewModel();

            pagesMap = new Dictionary<string, ObservableObject>()
            {
                { StaticPageNamespace.HomePageDescriptor, HomePage },
                { StaticPageNamespace.PackManagerDescriptor, PackManagerPage },
                { StaticPageNamespace.FlowManagerDescriptor, FlowManagerPage }
            };

            ViewModel = new MainPageViewModel(HomePage, MainWindowState, this);

            PackManagerPage.Parent = ViewModel;

            DataContext = ViewModel;
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public ObservableObject? TryGetPage(string pageName)
        {
            if (pagesMap.TryGetValue(pageName, out ObservableObject obj)) return obj;
            return null;
        }
    }
}
