using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Neat.Extensions;
using System;
using System.IO;
using System.Windows.Input;

namespace Neat.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        public FileInfo Model { get; }

        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private float size;
        [ObservableProperty]
        private string type;

        public ICommand Remove { get; }

        public event Action OnRemove;

        public FileViewModel(FileInfo model)
        {
            Model = model;

            Name = model.Name;
            Size = model.Length.ToMegaBytes();
            Type = model.Extension[1..];

            Remove = new RelayCommand(OnRemoveInvoked);
        }

        private void OnRemoveInvoked()
        {
            OnRemove?.Invoke();
            Model.Delete();
        }
    }
}
