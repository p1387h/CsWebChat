using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.LoginModule.Services;
using CsWebChat.WpfClient.SharedWebLogic.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
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
    class RegisterViewModel : BindableBase
    {
        // Header for TabControl display.
        private readonly string _header = "Register";
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
        
        public ICommand ButtonRegister { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly AuthenticationService _authenticationService;

        public RegisterViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, AuthenticationService authenticationService)
        {
            if (container == null || eventAggregator == null
                || logger == null || authenticationService == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._authenticationService = authenticationService;

            ButtonRegister = new DelegateCommand(async () => { await ButtonRegisterClicked(); });
            PasswordChangedCommand = new DelegateCommand<PasswordBox>((box) => { Password = box.SecurePassword; });
        }

        private async Task ButtonRegisterClicked()
        {
            var registered = await this._authenticationService.RegisterUser(new User());


            throw new NotImplementedException();
        }
    }
}
