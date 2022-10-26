using Microsoft.Extensions.DependencyInjection;
using Neat.Cloud;

namespace Neat.Extensions
{
    public static class AppExtensions
    {
        public static ICloudService GetCloudService(this App app)
        {
            return app.Services.GetService<ICloudService>();
        }
    }
}
