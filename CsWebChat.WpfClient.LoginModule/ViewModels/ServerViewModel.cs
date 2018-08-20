using CsWebChat.WpfClient.LoginModule.Events;
using CsWebChat.WpfClient.SharedWebLogic.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsWebChat.WpfClient.LoginModule.ViewModels
{
    class ServerViewModel : BindableBase
    {
        // Header for TabControl display.
        private readonly string _header = "Server";
        public string Header
        {
            get { return _header; }
        }

        private ObservableCollection<string> _serverAddresses;
        public ObservableCollection<string> ServerAddresses
        {
            get { return _serverAddresses; }
            set { _serverAddresses = value; }
        }

        private string _newServerAddress;
        public string NewServerAddress
        {
            get { return _newServerAddress; }
            set { SetProperty<string>(ref _newServerAddress, value); }
        }

        public ICommand ButtonAdd { get; set; }

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly AddressStorage _addressStorage;

        public ServerViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, AddressStorage addressStorage)
        {
            if (container == null || eventAggregator == null
                || logger == null || addressStorage == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._addressStorage = addressStorage;

            // Only a shallow copy! New elements must also be added towards the model instance.
            ServerAddresses = new ObservableCollection<string>(this._addressStorage.Servers);

            ButtonAdd = new DelegateCommand(async () => { await this.ButtonAddClicked(); });
        }

        private async Task ButtonAddClicked()
        {
            if(!String.IsNullOrEmpty(NewServerAddress))
            {
                ServerAddresses.Add(NewServerAddress);
                this._addressStorage.Servers.Add(NewServerAddress);
                NewServerAddress = null;

                // Notify all other viewmodels that a new server was added.
                // -> The model does NOT implement INotifyPropertyChanged!
                this._eventAggregator.GetEvent<ServerAddedEvent>()
                    .Publish();
            }
        }
    }
}
