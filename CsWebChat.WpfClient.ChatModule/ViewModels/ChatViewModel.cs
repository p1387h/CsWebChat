using CsWebChat.WpfClient.ChatModule.Events;
using CsWebChat.WpfClient.ChatModule.Models;
using CsWebChat.WpfClient.Regions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CsWebChat.WpfClient.ChatModule.ViewModels
{
    class ChatViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { SetProperty<ObservableCollection<Message>>(ref _messages, value); }
        }

        private string this_message;
        public string Message
        {
            get { return this_message; }
            set { SetProperty<string>(ref this_message, value); }
        }

        private string _chatPartnerName;
        public string ChatPartnerName
        {
            get { return _chatPartnerName; }
            private set { SetProperty<string>(ref _chatPartnerName, value); }
        }

        public ICommand ButtonSend { get; set; }

        private HubConnection _connection;

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly IRegionManager _regionManager;

        public ChatViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, IRegionManager regionManager)
        {
            if (container == null || eventAggregator == null
                || logger == null || regionManager == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._regionManager = regionManager;

            Messages = new ObservableCollection<Message>();

            ButtonSend = new DelegateCommand(async () => { await this.ButtonSendClicked(); });

            this._eventAggregator.GetEvent<MessageReceivedEvent>()
                .Subscribe(this.MessageReceivedEventHandler, ThreadOption.UIThread, false, this.MessageReceivedEventFilter);
        }

        private async Task ButtonSendClicked()
        {
            if (!string.IsNullOrEmpty(Message) && ChatPartnerName != null)
            {
                await this._connection.SendAsync("SendMessageTo", ChatPartnerName, Message);
                this.Message = null;
            }
        }

        private void MessageReceivedEventHandler(Message message)
        {
            // Ensure the correct representation in the view.
            if(message.Sender.Name.Equals(ChatPartnerName))
            {
                message.IsFromChatPartner = true;
            }

            Messages.Add(message);
        }

        private bool MessageReceivedEventFilter(Message message)
        {
            return !String.IsNullOrEmpty(ChatPartnerName)
                && (message.Sender.Name.Equals(ChatPartnerName) || message.Receiver.Name.Equals(ChatPartnerName));
        }


        // INavigationAware:
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Keep the views for existing partners.
            var partnerName = navigationContext.Parameters["partnerName"];
            return ChatPartnerName != null && ChatPartnerName.Equals(partnerName);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ChatPartnerName = navigationContext.Parameters["partnerName"] as string;
            this._connection = (HubConnection)this._regionManager.Regions[MainWindowRegionNames.MAIN_REGION].Context;
        }
    }
}
