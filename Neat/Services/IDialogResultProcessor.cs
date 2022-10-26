using Neat.ViewModels;

namespace Neat.Services
{
    public interface IDialogResultProcessor
    {
        void AddHandler(DialogResultHandler handler);
        void ProcessResult(DialogInputResult result);
    }
}
