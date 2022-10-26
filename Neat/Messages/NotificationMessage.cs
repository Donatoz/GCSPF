using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Neat.ViewModels;
using System;
using System.Windows.Input;

namespace Neat.Messages
{
    public class NotificationMessage
    {
        public string Message { get; }
        public string Title { get; }
        public string Color { get; }
        public NotificationOption[] Options { get; }

        public NotificationMessage(string message, params NotificationOption[] options)
        {
            Message = message;
            Title = "Notification";
            Color = "GreyGreenColor";
            Options = options ?? Array.Empty<NotificationOption>();
        }

        public NotificationMessage(string message, string title, string color, params NotificationOption[] options) : this(message, options)
        {
            Title = title;
            Color = color;
        }

        public virtual NotificationViewModel CreateViewModel()
        {
            return new NotificationViewModel(Message, Title, Color, Options);
        }

        public static readonly NotificationMessage NotImplementedMessage = 
            new NotificationMessage("This feature is not implemented yet.", options: NotificationOption.ConfirmOption);

        public static NotificationMessage Error(string message)
        {
            return new NotificationMessage(message, "Error", "GreyRedColor", NotificationOption.ConfirmOption);
        }

        public static NotificationMessage Error(Exception e)
        {
            return Error($"{e.Message}\n\nStack trace:\n{e.StackTrace}");
        }
    }

    public class NotificationCloseMessage { }

    public class NotificationOption
    {
        public string Label { get; }
        public ICommand Command { get; }

        public NotificationOption(string label, ICommand command)
        {
            Label = label;
            Command = command;
        }

        public static readonly NotificationOption ConfirmOption = new NotificationOption("Confirm", new RelayCommand(() => WeakReferenceMessenger.Default.Send(new NotificationCloseMessage())));
    }
}
