using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public abstract class DialogViewModel : ObservableObject
    {
        public ICommand Confirm { get; }
        public ICommand Cancel { get; }

        private readonly AutoResetEvent confirmHandler;
        private ExitState state;

        public DialogViewModel()
        {
            Confirm = new RelayCommand(OnConfirm);
            Cancel = new RelayCommand(OnCancel);

            confirmHandler = new AutoResetEvent(false);
        }

        public async Task<DialogInputResult> RequestInput()
        {
            confirmHandler.WaitOne();
            return OnResult();
        }

        protected abstract DialogInputResult CreateResult(ExitState state);

        private DialogInputResult OnResult()
        {
            return state == ExitState.Cancel ? new DialogCancelResult() : CreateResult(state);
        }

        protected void OnConfirm()
        {
            state = ExitState.Confirm;
            confirmHandler.Set();
        }

        protected void OnCancel()
        {
            state = ExitState.Cancel;
            confirmHandler.Set();
        }

        public enum ExitState
        {
            Confirm,
            Cancel
        }
    }

    public class DialogInputResult
    {
        public bool Success { get; }
        public string? Error { get; }

        public DialogInputResult(bool success, string? error = null)
        {
            Success = success;
            Error = error;
        }
    }

    public class DialogCancelResult : DialogInputResult
    {
        public DialogCancelResult() : base(true, null)
        {
        }
    }
}
