using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Neat.Extensions;
using Neat.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Neat.Models
{
    public partial class Package : ObservableObject
    {
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string version;
        [ObservableProperty]
        private string id;

        private List<PackageSource> sources;
        public IReadOnlyList<PackageSource> Sources => sources;

        public List<string> Exports { get; set; }

        [JsonIgnore]
        private List<FileInfo> files;
        [JsonIgnore]
        public IReadOnlyList<FileInfo> Files => files;

        [JsonIgnore]
        public string SerializableName => Name.Replace(' ', '_');
        [JsonIgnore]
        public float TotalSize => Files.Sum(f => f.Length.ToMegaBytes());
        [JsonIgnore]
        public int FilesCount => Files.Count;

        private FileSystemWatcher fileWatcher;

        public event Action FilesChanged;
        public event Action SourcesChanged;
        public event Action ExportsChanged;

        public Package(string name, string id)
        {
            Name = name;
            Version = "1.0.0";
            this.id = id;

            Exports = new List<string>();
            files = new List<FileInfo>();
            sources = new List<PackageSource>();

            if (sources.Count == 0)
            {
                AddSource(App.Current.Services.GetService<IPathProvider>().GetPackageResourcesPath(this), PackageSourceType.LocalDirectory, false, false);
            }

            FetchFiles();
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            fileWatcher = new FileSystemWatcher(App.Current.Services.GetService<IPathProvider>().GetPackageResourcesPath(this));
            fileWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            fileWatcher.IncludeSubdirectories = true;

            fileWatcher.Created += OnFilesChanged;
            fileWatcher.Deleted += OnFilesChanged;
            fileWatcher.Renamed += OnFilesChanged;
            fileWatcher.Changed += OnFilesChanged;

            fileWatcher.EnableRaisingEvents = true;
        }

        public bool Save()
        {
            var savePath = App.Current.Services.GetService<IPathProvider>().PackagesPath + $"/{Id}.json";
            return App.Current.Services.GetService<IPackageProvider>().SavePackage(this, savePath);
        }

        public void RemoveFile(FileInfo file)
        {
            files.Remove(file);
        }

        public void FetchFiles(bool notify = false)
        {
            files.Clear();

            foreach (var s in sources)
            {
                FetchSource(s);
            }

            if (notify)
            {
                FilesChanged?.Invoke();
            }
        }

        public void FetchSource(PackageSource source)
        {
            var files = source.Fetch();
            this.files.AddRange(files);
        }

        public PackageSource? AddSource(string source, PackageSourceType type, bool removable = true, bool editable = true)
        {
            if (sources.Any(s => s.Path == source)) return null;

            var s = new PackageSource(type, source, removable, editable);
            sources.Add(s);

            return s;
        }

        public void AddExport(string path)
        {
            Exports.Add(path);
            ExportsChanged?.Invoke();
        }

        public void RemoveExport(string path)
        {
            Exports.Remove(path);
            ExportsChanged?.Invoke();
        }

        public Task InvokeExport(string path)
        {
            return Task.Run(() => SingleExport(path));
        }

        private async Task SingleExport(string path)
        {
            await Task.Delay(500);

            for (int i = Files.Count - 1; i >= 0; i--)
            {
                FileInfo? f = Files[i];
                using (var source = File.Open(f.FullName, FileMode.Open))
                {
                    using (var destination = File.Create(path + $"\\{f.Name}"))
                    {
                        await source.CopyToAsync(destination);
                    }
                }
            }
        }

        public void RemoveSource(PackageSource source)
        {
            sources.Remove(source);
        }

        public void AddFiles(params FileInfo[] files)
        {
            if (files.Length == 0) return;

            this.files.AddRange(files.ToArray());
            FilesChanged?.Invoke();
        }

        private void OnFilesChanged(object sender, FileSystemEventArgs e)
        {
            FetchFiles();
            FilesChanged?.Invoke();
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
        }
    }
}
