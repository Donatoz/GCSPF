using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Neat.Messages;
using Neat.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Neat.ViewModels
{
    public partial class MainPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableObject currentPage;
        [ObservableProperty]
        private NotificationViewModel notification;
        [ObservableProperty]
        private PopupViewModel popup;
        [ObservableProperty]
        private DialogViewModel dialog;
        [ObservableProperty]
        private AnticipantViewModel anticipant;

        public bool PopupsEnabled { get; set; } = true;

        private readonly Queue<PopupMessage> popupQueue;
        private readonly IPageProvider pageProvider;

        public MainPageViewModel(ObservableObject currentPage, Window mainWindow, IPageProvider pageProvider)
        {
            WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, OnPageChanged);
            WeakReferenceMessenger.Default.Register<NotificationMessage>(this, OnNotification);
            WeakReferenceMessenger.Default.Register<NotificationCloseMessage>(this, OnNotificationClose);
            WeakReferenceMessenger.Default.Register<PopupMessage>(this, OnPopup);
            WeakReferenceMessenger.Default.Register<AnticipantMessage>(this, OnAnticipant);

            WeakReferenceMessenger.Default.Register<CreatePackageDialogMessage>(this, OnDialog);
            WeakReferenceMessenger.Default.Register<AddFilesDialogMessage>(this, OnDialog);
            WeakReferenceMessenger.Default.Register<FetchPackageDialogMessage>(this, OnDialog);

            popupQueue = new Queue<PopupMessage>();

            this.pageProvider = pageProvider;
            this.currentPage = currentPage;

            foreach (var e in App.Current.InitializationErrors)
            {
                WeakReferenceMessenger.Default.Send(NotificationMessage.Error(e));
            }
        }

        private void OnPageChanged(object sender, PageChangedMessage message)
        {
            CurrentPage = pageProvider.TryGetPage(message.Value);
        }

        private void OnNotification(object sender, NotificationMessage message)
        {
            Notification = message.CreateViewModel();
        }

        private void OnAnticipant(object sender, AnticipantMessage message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (Anticipant != null)
                {
                    Anticipant.Routines.Add(message.Routine);
                }
                else
                {
                    Anticipant = new AnticipantViewModel(message.Routine);
                }
            });

            message.OnEnded += OnAnticipantRoutineEnd;
        }

        private void OnAnticipantRoutineEnd(AnticipantMessage message)
        {
            message.OnEnded -= OnAnticipantRoutineEnd;

            if (Anticipant == null) return;

            App.Current.Dispatcher.Invoke(() =>
            {
                Anticipant.Routines.Remove(message.Routine);

                if (Anticipant.IsEmpty)
                {
                    Anticipant = null;
                }
            });
        }

        private void OnPopup(object sender, PopupMessage message)
        {
            if (!PopupsEnabled) return;

            popupQueue.Enqueue(message);

            if (Popup == null)
            {
                ShowPopup(popupQueue.Dequeue());
            }
        }

        private async void ShowPopup(PopupMessage message)
        {
            Popup = message.CreateViewModel();

            await Task.Delay(100);

            Popup.Show();

            await Task.Delay(message.Lifetime);

            Popup.Hide();

            await Task.Delay(400);

            if (popupQueue.Count > 0)
            {
                ShowPopup(popupQueue.Dequeue());
            }
            else
            {
                Popup = null;
            }
        }

        private void OnDialog(object sender, DialogMessage message)
        {
            if (Dialog != null) return;

            Task.Run(() => ShowDialog(message));
        }

        private async void ShowDialog(DialogMessage message)
        {
            Dialog = message.CreateViewModel();
            var success = false;

            while (!success)
            {
                var result = await Dialog.RequestInput();

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!result.Success)
                    {
                        WeakReferenceMessenger.Default.Send(new PopupMessage(result.Error ?? "Please, check your input.", "Close", 20, "GreyRedColor"));
                    }
                    else
                    {
                        App.Current.Services.GetService<IDialogResultProcessor>().ProcessResult(result);
                        Dialog = null;
                    }
                });

                success = result.Success;
            }
        }

        private void OnNotificationClose(object sender, NotificationCloseMessage message)
        {
            Notification = null;
        }
    }
}
