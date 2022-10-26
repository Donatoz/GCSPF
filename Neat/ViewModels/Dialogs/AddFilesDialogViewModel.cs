using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class AddFilesDialogViewModel : DialogViewModel
    {
        [ObservableProperty]
        private bool copy;
        public ICommand SelectFiles { get; }

        private FileInfo[] selectedFiles;

        public AddFilesDialogViewModel()
        {
            Copy = true;
            SelectFiles = new RelayCommand(OnSelectFiles);
        }

        protected override DialogInputResult CreateResult(ExitState state)
        {
            return new AddFilesDialogResult(selectedFiles, Copy);
        }

        private void OnSelectFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            var res = dialog.ShowDialog();

            if (res == true)
            {
                selectedFiles = new FileInfo[dialog.FileNames.Length];

                for (var i = 0; i < dialog.FileNames.Length; i++)
                {
                    selectedFiles[i] = new FileInfo(dialog.FileNames[i]);
                }
            }
        }
    }

    public class AddFilesDialogResult : DialogInputResult
    {
        public FileInfo[] Files { get; }
        public bool Copy { get; }

        public AddFilesDialogResult(FileInfo[] files, bool copy, string? error = null) : base(true, error)
        {
            Files = files;
            Copy = copy;
        }
    }
}
