using CsWebChat.WpfClient.ChatModule.Events;
using CsWebChat.WpfClient.ChatModule.Models;
using CsWebChat.WpfClient.ChatModule.Views;
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
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CsWebChat.WpfClient.ChatModule.ViewModels
{
    class UserViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty<ObservableCollection<User>>(ref _users, value); }
        }

        private ConnectionState _connectionState;
        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty<ConnectionState>(ref _connectionState, value); }
        }

        public ICommand ButtonUserName { get; set; }

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

            this._eventAggregator.GetEvent<WebsocketConnectionStateEvent>()
                .Subscribe(this.HandleWebsocketConnectionStateEvent, ThreadOption.BackgroundThread);

            this.ButtonUserName = new DelegateCommand<string>(this.ButtonUserNameClicked);
        }

        private async void HandleWebsocketConnectionStateEvent(WebSocketState state)
        {
            if (state == WebSocketState.Open)
            {
                ConnectionState = ConnectionState.Connected;
                await this._connection.SendAsync("RequestOtherUserStates");
            }
            else if (state == WebSocketState.Closed)
            {
                ConnectionState = ConnectionState.Disconnected;
                await Application.Current.Dispatcher.InvokeAsync(() => { Users.Clear(); });
            }
        }

        private void ButtonUserNameClicked(string userName)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("partnerName", userName);

            this._regionManager.RequestNavigate(ChatModuleRegionNames.CHAT_REGION,
                new Uri(nameof(ChatView), UriKind.Relative),
                navigationParameters);
        }


        // IRegionMemberLifetime:
        public bool KeepAlive
        {
            get { return false; }
        }


        // INavigationAware:
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Force instantiation and requesting updated list of other users.
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._connection = (HubConnection)this._regionManager.Regions[MainWindowRegionNames.MAIN_REGION].Context;
            this._connection.On<IEnumerable<string>, UserState>("NotifyUsersStateChangesAsync", this.HandleStateChanges);
        }

        private async void HandleStateChanges(IEnumerable<string> names, UserState state)
        {
            foreach (var name in names)
            {
                await this.HandleStateChangeAsync(name, state);
            }
        }

        private async Task HandleStateChangeAsync(string name, UserState state)
        {
            if (state == UserState.Online)
            {
                var newUser = new User() { Name = name };
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (!Users.Contains(newUser))
                        {
                            Users.Add(newUser);
                        }
                    });
            }
            else if (state == UserState.Offline)
            {
                var toRemove = Users.FirstOrDefault(x => x.Name.Equals(name));
                await Application.Current.Dispatcher.InvokeAsync(() => { Users.Remove(toRemove); });
            }
        }
    }
}
