using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.LoginModule.Services;
using CsWebChat.WpfClient.SharedWebLogic.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CsWebChat.WpfClient.LoginModule.ViewModels
{
    class LoginViewModel : BindableBase
    {
        // Header for TabControl display.
        private readonly string _header = "Login";
        public string Header
        {
            get { return _header; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty<string>(ref _name, value); }
        }

        public SecureString Password { get; set; }
        public List<string> ServerAddresses { get { return this._addressStorage.Servers; } }

        private string _selectedServerAddress;
        public string SelectedServerAddress
        {
            get { return _selectedServerAddress; }
            set
            {
                SetProperty<string>(ref _selectedServerAddress, value);
                this._addressStorage.ServerAddress = value;
            }
        }

        public ICommand ButtonLogin { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly AddressStorage _addressStorage;

        public LoginViewModel(IUnityContainer container, IEventAggregator eventAggregator, 
            ILoggerFacade logger, AuthenticationService authenticationService,
            AddressStorage addressStorage)
        {
            if (container == null || eventAggregator == null 
                || logger == null || authenticationService == null
                || addressStorage == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._authenticationService = authenticationService;
            this._addressStorage = addressStorage;

            ButtonLogin = new DelegateCommand(async () => { await ButtonRegisterClicked(); });
            PasswordChangedCommand = new DelegateCommand<PasswordBox>((box) => { Password = box.SecurePassword; });
        }

        private async Task ButtonRegisterClicked()
        {
            var registered = await this._authenticationService.LoginUser(new User());



            throw new NotImplementedException();
        }
    }
}
