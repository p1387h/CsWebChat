using CsWebChat.WpfClient.ChatModule.Models;
using CsWebChat.WpfClient.Regions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Practices.Unity;
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

namespace CsWebChat.WpfClient.ChatModule.ViewModels
{
    class UserViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty<ObservableCollection<User>>(ref _users, value); }
        }

        private HubConnection _connection;

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly IRegionManager _regionManager;

        public UserViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, IRegionManager regionManager)
        {
            if (container == null || eventAggregator == null
                || logger == null || regionManager == null)
                throw new ArgumentException();

            this._container = container;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._regionManager = regionManager;

            Users = new ObservableCollection<User>();
        }


        // INavigationAware:
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._connection = (HubConnection)this._regionManager.Regions[MainWindowRegionNames.MAIN_REGION].Context;
            this._connection.On<string, UserState>("NotifyUserStateChange", this.HandleStateChange);
        }

        private async void HandleStateChange(string name, UserState state)
        {
            if (state == UserState.Online)
            {
                var newUser = new User() { Name = name };
                await Application.Current.Dispatcher.InvokeAsync(() => { Users.Add(newUser); });
            }
            else if (state == UserState.Offline)
            {
                var toRemove = Users.FirstOrDefault(x => x.Name.Equals(name));
                await Application.Current.Dispatcher.InvokeAsync(() => { Users.Remove(toRemove); });
            }
        }
    }
}
