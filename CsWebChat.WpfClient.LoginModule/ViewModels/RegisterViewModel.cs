using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.ViewModels
{
    class RegisterViewModel
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;

        public RegisterViewModel(IUnityContainer container, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            if (container == null || eventAggregator == null || logger == null)
                throw new ArgumentException();

            _container = container;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }
    }
}
