using CsWebChat.WpfClient.ChatModule.Views;
using CsWebChat.WpfClient.Regions;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.ViewModels
{
    class ChatSplitViewModel : BindableBase, INavigationAware
    {
        // Set as RegionContext in order to make the connection available to all 
        // other hostet child views.
        public HubConnection ConnectionRegionContext { get { return this._connection; } }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly HubConnection _connection;

        public ChatSplitViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            ILoggerFacade logger, HubConnection connection)
        {
            if (regionManager == null || eventAggregator == null
                || logger == null || connection == null)
                throw new ArgumentException();

            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._connection = connection;
        }


        // INavigationAware:
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Task.Run(async () => { await this._connection.StartAsync(); });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Always return false in order to force a new instantiation.
            // -> Injecting a new HubConnection is necessary since the address might
            // change.
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Task.Run(async () => { await ConnectionRegionContext.StopAsync(); });
        }
    }
}
