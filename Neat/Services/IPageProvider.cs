using CommunityToolkit.Mvvm.ComponentModel;

namespace Neat.Services
{
    public interface IPageProvider
    {
        ObservableObject? TryGetPage(string pageName);
    }
}
