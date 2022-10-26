using CommunityToolkit.Mvvm.ComponentModel;
using Neat.Models;
using System;

namespace Neat.ViewModels
{
    public partial class CreatePackageDialogViewModel : DialogViewModel
    {
        [ObservableProperty]
        private string packageName;

        protected override DialogInputResult CreateResult(ExitState state)
        {
            if (packageName != null && packageName.Length > 0)
            {
                return new CreatePackageDialogResult(new Package(PackageName, Guid.NewGuid().ToString()));
            }
            else
            {
                return new CreatePackageDialogResult(null, "Package name is required");
            }
        }
    }

    public class CreatePackageDialogResult : DialogInputResult
    {
        public Package? Package { get; }

        public CreatePackageDialogResult(Package? package, string? error = null) : base(package != null, error)
        {
            Package = package;
        }
    }
}
