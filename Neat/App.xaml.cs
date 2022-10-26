using Microsoft.Extensions.DependencyInjection;
using Neat.Cloud;
using Neat.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Neat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; set; }
        public new static App Current => (App)Application.Current;

        private List<Exception> initializationErrors = new List<Exception>();

        public IReadOnlyList<Exception> InitializationErrors => initializationErrors;

        public App()
        {
            Services = ConfigureServices();

            InitializeComponent();
        }

        public T GetResource<T>(string key)
        {
            return (T) TryFindResource(key);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var googleCloud = new GoogleCloudService();
            ConnectCloudService(googleCloud);

            services.AddSingleton<IPackageProvider>(new FilePackageProvider());
            services.AddSingleton<IPathProvider>(new DefaultPathProvider());
            services.AddSingleton<IDialogResultProcessor>(new DefaultDialogResultProcessor());
            services.AddSingleton<ICloudService>(googleCloud);

            return services.BuildServiceProvider();
        }

        private async void ConnectCloudService(GoogleCloudService service)
        {
            var error = await service.Connect();
            if (error != null)
            {
                initializationErrors.Add(error);
            }
        }
    }
}
