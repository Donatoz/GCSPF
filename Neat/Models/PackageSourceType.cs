using System.Text.RegularExpressions;

namespace Neat.Models
{
    public enum PackageSourceType
    {
        LocalDirectory,
        ExternalRepository
    }

    public static class PackageSourceExtensions
    {
        public static string GetName(this PackageSourceType type)
        {
            return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
        }
    }
}
