using CommunityToolkit.Mvvm.ComponentModel;
using Neat.Services;
using Neat.Utils;

namespace Neat.ViewModels
{
    public partial class VaryingServiceViewModel : ObservableObject
    {
        public NotifiableProperty<bool> IsAvailable { get; }

        public VaryingServiceViewModel(IVaryingService model)
        {
            IsAvailable = new NotifiableProperty<bool>(model.IsAvailable);
        }
    }
}
