using Google.Cloud.Storage.V1;
using Neat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neat.Cloud
{
    public class GoogleCloudService : ICloudService
    {
        private const string key = "neatapi-391194c821ea.json";

        private StorageClient client;

        public ReactiveProperty<bool> IsAvailable { get; } = new ReactiveProperty<bool>(false);

        public Task<Exception?> Connect()
        {
            return Task.Run(BeginConnect);
        }

        private async Task<Exception?> BeginConnect()
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + $"\\{key}";
                var json = File.ReadAllText(path);

                var builder = new StorageClientBuilder();
                builder.JsonCredentials = json;



                client = await builder.BuildAsync();
                IsAvailable.Value = true;
                return null;
            }
            catch (FileNotFoundException e)
            {
                IsAvailable.Value = false;
                return e;
            }
        }

        public void Disconnect()
        {
            IsAvailable.Value = false;
            client?.Dispose();
        }

        public Task<FileInfo?> Fetch(string root, string fileName, string destination)
        {
            if (!IsAvailable.Value) return null;

            return Task.Run(() => FetchFile(root, fileName, destination));
        }

        private async Task<FileInfo?> FetchFile(string root, string filename, string destination)
        {
            var bucket = await client.GetBucketAsync(root);

            using (var stream = File.OpenWrite(destination))
            {
                var handle = await client.DownloadObjectAsync(bucket.Name, filename, stream);

                return new FileInfo(destination);
            }
        }

        public Task<string?> UploadFile(string destination, FileInfo file)
        {
            return Task.Run(() => UploadFileToDrive(destination, file));
        }

        private async Task<string?> UploadFileToDrive(string destination, FileInfo file)
        {
            if (client != null)
            {
                var stream = new MemoryStream();
                await client.DownloadObjectAsync("mainhub", "canvas.txt", stream);


            }

            return null;
        }

        public Task<IEnumerable<string>> List(string root, string prefix, int maxFilesAmount)
        {
            if (!IsAvailable.Value) return null;

            return Task.Run(() => ListFiles(root, prefix, maxFilesAmount));
        }

        private async Task<IEnumerable<string>> ListFiles(string root, string prefix, int maxFilesAmount)
        {
            var files = new List<string>();

            var bucket = await client.GetBucketAsync(root);

            var listQuery = await client.ListObjectsAsync(bucket.Name, prefix).ReadPageAsync(maxFilesAmount);

            return listQuery.Select(o => o.Name.Replace(prefix, string.Empty)).Where(s => s.Length > 0);
        }
    }
}
