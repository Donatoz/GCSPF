using System.Collections.ObjectModel;

namespace Neat.ViewModels
{
    public class AnticipantViewModel
    {
        public ObservableCollection<string> Routines { get; }

        public bool IsEmpty => Routines.Count == 0;

        public AnticipantViewModel(string initialRoutine)
        {
            Routines = new ObservableCollection<string>();
            
            Routines.Add(initialRoutine);
        }
    }
}
