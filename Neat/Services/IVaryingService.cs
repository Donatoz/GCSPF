using Neat.Utils;

namespace Neat.Services
{
    public interface IVaryingService
    {
        ReactiveProperty<bool> IsAvailable { get; }
    }
}
