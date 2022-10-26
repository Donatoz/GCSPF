using Neat.Models;

namespace Neat.Services
{
    public interface IPackageProvider
    {
        Package? FetchPackage(string path);
        bool SavePackage(Package package, string path);
    }
}
