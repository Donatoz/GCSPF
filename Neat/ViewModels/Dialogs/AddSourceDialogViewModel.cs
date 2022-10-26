using CommunityToolkit.Mvvm.ComponentModel;

namespace Neat.ViewModels.Dialogs
{
    public partial class AddSourceDialogViewModel : DialogViewModel
    {
        [ObservableProperty]
        private string path;

        protected override DialogInputResult CreateResult(ExitState state)
        {
            if (Path != null & Path.Length > 0)
            {
                return new AddSourceDialogResult(Path);
            }
            else
            {
                return new AddSourceDialogResult(null, "Path must be specified");
            }
        }
    }

    public class AddSourceDialogResult : DialogInputResult
    {
        public string Path { get; }

        public AddSourceDialogResult(string path, string? error = null) : base(path != null && path.Length > 0, error)
        {
            Path = path;
        }
    }
}
