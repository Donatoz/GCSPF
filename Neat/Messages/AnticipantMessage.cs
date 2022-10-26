using System;

namespace Neat.Messages
{
    public class AnticipantMessage : AppMessage
    {
        public string Routine { get; }

        public event Action<AnticipantMessage> OnEnded;

        public AnticipantMessage(string routine)
        {
            Routine = routine;
        }

        public void End()
        {
            OnEnded?.Invoke(this);
        }
    }
}
