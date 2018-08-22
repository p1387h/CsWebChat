using CsWebChat.WpfClient.Events;
using CsWebChat.WpfClient.LoginModule.Events;
using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.LoginModule.Services;
using CsWebChat.WpfClient.WebLogicModule.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
            set
            {
                SetProperty<string>(ref _name, value);

                // Ensure that the login button triggers accordingly.
                if (String.IsNullOrEmpty(_name))
                {
                    EnableLoginButton = false;
                }
                else if (Password?.Length > 0)
                {
                    EnableLoginButton = true;
                }
            }
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

        private bool _enableLoginButton;
        public bool EnableLoginButton
        {
            get { return _enableLoginButton; }
            set { SetProperty<bool>(ref _enableLoginButton, value); }
        }

        private ObservableCollection<string> _errorMessages = new ObservableCollection<string>();
        public ObservableCollection<string> ErrorMessages
        {
            get { return _errorMessages; }
            set { SetProperty<ObservableCollection<string>>(ref _errorMessages, value); }
        }

        private bool _enableProgressRing;
        public bool EnableProgressRing
        {
            get { return _enableProgressRing; }
            set { SetProperty<bool>(ref _enableProgressRing, value); }
        }

        public ICommand ButtonLogin { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly AddressStorage _addressStorage;
        private readonly PasswordHashService _passwordHashService;

        public LoginViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, AuthenticationService authenticationService,
            AddressStorage addressStorage, PasswordHashService passwordHashService)
        {
            if (container == null || eventAggregator == null
                || logger == null || authenticationService == null
                || addressStorage == null || passwordHashService == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._authenticationService = authenticationService;
            this._addressStorage = addressStorage;
            this._passwordHashService = passwordHashService;

            // Trigger the getter once in order to force the view to load 
            // any existing selected addresses.
            SelectedServerAddress = this._addressStorage.ServerAddress;

            // Ensure that the visible servers are updated.
            this._eventAggregator.GetEvent<ServerAddedEvent>()
                .Subscribe(() => { RaisePropertyChanged(nameof(ServerAddresses)); }, ThreadOption.UIThread);

            ButtonLogin = new DelegateCommand(async () => { await this.ButtonLoginClicked(); });
            PasswordChangedCommand = new DelegateCommand<PasswordBox>(async (box) => { await this.PasswordChangedFired(box); });
        }

        private async Task ButtonLoginClicked()
        {
            ErrorMessages.Clear();

            try
            {
                if (!String.IsNullOrEmpty(SelectedServerAddress) && Uri.IsWellFormedUriString(SelectedServerAddress, UriKind.Absolute))
                {
                    EnableProgressRing = true;

                    var user = new User()
                    {
                        Name = Name,
                        Password = this._passwordHashService.HashSecureString(Password)
                    };
                    var loginResult = await this._authenticationService.LoginUser(user);

                    if (loginResult.Success)
                    {
                        this._eventAggregator.GetEvent<LoginSuccessEvent>()
                            .Publish();
                    }
                    else
                    {
                        ErrorMessages.Add(String.Format("Login unsuccessful. Reason: {0}", loginResult.StatusCode));

                        // Show the response of the server to the user.
                        if (loginResult.Response != null)
                        {
                            var outputMap = "Field: {0}, Value: {1}";

                            if (!String.IsNullOrEmpty(loginResult.Response.Name))
                                ErrorMessages.Add(String.Format(outputMap, nameof(Name), loginResult.Response.Name));
                            if (!String.IsNullOrEmpty(loginResult.Response.Password))
                                ErrorMessages.Add(String.Format(outputMap, nameof(Password), loginResult.Response.Password));
                        }

                    }
                }
                else
                {
                    ErrorMessages.Add("Please use a valid URL.");
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessages.Add("Server could not be reached.");
            }
            finally
            {
                EnableProgressRing = false;
            }
        }

        private async Task PasswordChangedFired(PasswordBox box)
        {
            Password = box.SecurePassword;

            // Ensure that the login button triggers accordingly.
            if (box.SecurePassword == null || box.SecurePassword.Length == 0)
            {
                EnableLoginButton = false;
            }
            else if (!String.IsNullOrEmpty(Name))
            {
                EnableLoginButton = true;
            }
        }
    }
}
