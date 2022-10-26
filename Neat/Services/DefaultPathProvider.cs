using Neat.Models;
using System;
using System.IO;

namespace Neat.Services
{
    public class DefaultPathProvider : IPathProvider
    {
        public string PackagesPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Tempo\\Packages";
        public string BufferPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Tempo\\Buffer";

        public DefaultPathProvider()
        {
            if (!Directory.Exists(PackagesPath))
            {
                Directory.CreateDirectory(PackagesPath);
            }

            if (!Directory.Exists(BufferPath))
            {
                Directory.CreateDirectory(BufferPath);
            }
        }

        public string GetPackageMetaPath(Package package)
        {
            return PackagesPath + $"\\{package.Id}.json";
        }

        public string GetPackageResourcesPath(Package package)
        {
            return BufferPath + $"\\{package.Id}";
        }
    }
}
