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
using System.Windows.Input;

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

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty<string>(ref _message, value); }
        }

        public string ChatPartnerName { get; private set; }

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

            this.Messages = new ObservableCollection<Message>();

            this.ButtonSend = new DelegateCommand(async () => { await this.ButtonSendClicked(); });
        }

        private async Task ButtonSendClicked()
        {
            if(!string.IsNullOrEmpty(_message))
            {
                await this._connection.SendAsync("SendMessageTo", this.ChatPartnerName, this.Message);
                this.Message = null;
            }
        }

        private void HandleReceiveMessage(Message message)
        {
            this.Messages.Add(message);
        }


        // INavigationAware:
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Keep the views for existing partners.
            var partnerName = navigationContext.Parameters["partnerName"];
            return this.ChatPartnerName != null && this.ChatPartnerName.Equals(partnerName);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.ChatPartnerName = navigationContext.Parameters["partnerName"] as string;
            this._connection = (HubConnection)this._regionManager.Regions[MainWindowRegionNames.MAIN_REGION].Context;
            this._connection.On<Message>("ReceiveMessageAsync", this.HandleReceiveMessage);
        }
    }
}
