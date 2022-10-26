using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Neat.Cloud;
using Neat.Extensions;
using Neat.Messages;
using Neat.Services;
using Neat.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neat.Utils
{
    public class CloudPackageFetchMessageHandler
    {
        private List<AnticipantMessage> currentAnticipants = new List<AnticipantMessage>();
        private readonly IPackageHost host;

        public CloudPackageFetchMessageHandler(IPackageHost host)
        {
            this.host = host;
            App.Current.Services.GetService<IDialogResultProcessor>().AddHandler(new DialogResultHandler(typeof(FetchPackageDialogResult), OnInput));
        }

        private void OnInput(DialogInputResult message)
        {
            if (!message.Success) return;

            var c = message as FetchPackageDialogResult;
            var packageName = c.PackageName;

            FetchPackage(c.Root, packageName);
        }

        public Task FetchPackage(string root, string packageName)
        {
            return Task.Run(() => InvokeFetch(root, packageName));
        }

        private async void InvokeFetch(string root, string packageName)
        {
            try
            {
                var downloaded = 0;
                var rootPath = $"Packages/{packageName}";

                var metaPath = App.Current.Services.GetService<IPathProvider>().PackagesPath;
                var bufferPath = App.Current.Services.GetService<IPathProvider>().BufferPath;

                var rootMessage = new AnticipantMessage("Fetching package...");
                WeakReferenceMessenger.Default.Send(rootMessage);
                currentAnticipants.Add(rootMessage);

                var service = App.Current.GetCloudService();

                var metaMessage = new AnticipantMessage("Fetching meta");
                WeakReferenceMessenger.Default.Send(metaMessage);
                currentAnticipants.Add(metaMessage);

                var metaRes = await service.List(root, $"{rootPath}/Meta/", 100);
                var meta = metaRes.First();
                var metaId = meta[..^5];

                metaMessage.End();
                currentAnticipants.Remove(metaMessage);

                var resMessage = new AnticipantMessage("Fetching resources");
                WeakReferenceMessenger.Default.Send(resMessage);
                currentAnticipants.Add(resMessage);

                var resources = await service.List(root, $"{rootPath}/Buffer/", 1000);

                resMessage.End();
                currentAnticipants.Remove(resMessage);

                if (!File.Exists($"{metaPath}\\{meta}"))
                {
                    await DownloadFile(service, root, $"{rootPath}/Meta/{meta}", $"{metaPath}\\{meta}");
                    downloaded++;
                }

                if (!Directory.Exists(bufferPath + $"\\{metaId}"))
                {
                    Directory.CreateDirectory(bufferPath + $"\\{metaId}");
                }

                foreach (var r in resources)
                {
                    if (File.Exists($"{bufferPath}\\{metaId}\\{r}")) continue;
                    await DownloadFile(service, root, $"{rootPath}/Buffer/{r}", $"{bufferPath}\\{metaId}\\{r}");
                    downloaded++;
                }

                rootMessage.End();
                currentAnticipants.Remove(rootMessage);

                App.Current.Dispatcher.Invoke(() =>
                {
                    var package = App.Current.Services.GetService<IPackageProvider>().FetchPackage($"{metaPath}\\{meta}");

                    if (package != null)
                    {
                        host.IncludePackage(package);

                        if (downloaded > 0)
                        {
                            WeakReferenceMessenger.Default.Send(new PopupMessage("Package was successfully fetched", "Check", 20, "GreyGreenColor"));
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(new PopupMessage("Package is up to date", "Info", 10));
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(new PopupMessage("Package was loaded but was unnable to open", "Info", 12, "GreyRedColor"));
                    }
                });
            }
            catch (Exception e)
            {
                App.Current.Dispatcher.Invoke(() => WeakReferenceMessenger.Default.Send(NotificationMessage.Error(e)));
                currentAnticipants.ForEach(a => a.End());
                currentAnticipants.Clear();
            }
        }

        private async Task DownloadFile(ICloudService service, string root, string file, string dest)
        {
            var antipMessage = new AnticipantMessage($"Downloading {file.Split('/').Last()}");
            WeakReferenceMessenger.Default.Send(antipMessage);

            await service.Fetch(root, file, dest);

            antipMessage.End();
        }
    }
}
