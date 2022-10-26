using Neat.ViewModels;
using Neat.ViewModels.Dialogs;

namespace Neat.Messages
{
    public abstract class DialogMessage
    {
        public abstract DialogViewModel CreateViewModel();
    }

    public class CreatePackageDialogMessage : DialogMessage
    {
        public override DialogViewModel CreateViewModel()
        {
            return new CreatePackageDialogViewModel();
        }
    }

    public class AddFilesDialogMessage : DialogMessage
    {
        public override DialogViewModel CreateViewModel()
        {
            return new AddFilesDialogViewModel();
        }
    }

    public class FetchPackageDialogMessage : DialogMessage
    {
        public override DialogViewModel CreateViewModel()
        {
            return new FetchPackageDialogViewModel();
        }
    }
}
