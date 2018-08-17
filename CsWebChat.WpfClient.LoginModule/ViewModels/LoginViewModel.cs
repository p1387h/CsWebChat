using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        // Header for TabControl display.
        private readonly string _header = "Login";
        public string Header
        {
            get { return _header; }
        }

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;

        public LoginViewModel(IUnityContainer container, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            if (container == null || eventAggregator == null || logger == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
        }
    }
}
