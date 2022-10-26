using Neat.Models;

namespace Neat.Services
{
    public interface IPathProvider
    {
        string PackagesPath { get; set; }
        string BufferPath { get; set; }

        string GetPackageResourcesPath(Package package);
        string GetPackageMetaPath(Package package);
    }
}
