using CsWebChat.WpfClient.ChatModule.Views;
using CsWebChat.WpfClient.Regions;
using CsWebChat.WpfClient.WebLogicModule.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule
{
    public class ChatModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ChatModule(IUnityContainer container, IRegionManager regionManager)
        {
            if (container == null || regionManager == null)
                throw new ArgumentException();

            this._container = container;
            this._regionManager = regionManager;
        }

        public void Initialize()
        {
            this.InitializeServices();
            this.InitializeViews();
        }

        private void InitializeServices()
        {
            this._container.RegisterType<HubConnection>(new InjectionFactory(this.HubConnectionFactoryFunction));

            // Views registered for the possibility of INavigationAware returning false and
            // the RegionManager therefore requiring a new instance. The name is needed since
            // a Uri is used for resolving the type when navigating.
            this._container.RegisterType<object, ChatSplitView>(nameof(ChatSplitView));
            this._container.RegisterType<object, UserView>(nameof(UserView));
            this._container.RegisterType<object, ChatView>(nameof(ChatView));
        }

        private HubConnection HubConnectionFactoryFunction(IUnityContainer container)
        {
            var addressStorage = container.Resolve<AddressStorage>();
            var authenticationStorage = container.Resolve<AuthenticationStorage>();
            var antiforgeryStorage = container.Resolve<AntiforgeryStorage>();

            // Domain has to be set in order to prevent ArgurmentExceptions when adding
            // the Cookies.
            var target = new Uri(addressStorage.ServerAddress);
            var domain = target.Host;
            var authenticationCookie = new Cookie(authenticationStorage.AuthenticationCookieName,
                authenticationStorage.AuthenticationCookieContent)
            {
                Domain = domain
            };
            var antiforgeryCookie = new Cookie(antiforgeryStorage.AntiforgeryCookieName,
                antiforgeryStorage.AntiforgeryCookieContent)
            {
                Domain = domain
            };

            // The address of the server could may already end with a "/". In this case
            // another one is not needed.
            var combiner = (addressStorage.ServerAddress.EndsWith("/")) ? "" : "/";
            var address = String.Join(combiner, addressStorage.ServerAddress, "ws");

            // Ensure that the correct connection to the websocket is made and is always present 
            // when requesting an instance.
            var connection = new HubConnectionBuilder()
                .WithUrl(address, (options) =>
                {
                    options.Cookies.Add(authenticationCookie);
                    options.Cookies.Add(antiforgeryCookie);
                    options.Headers.Add(antiforgeryStorage.CsrfHeader, antiforgeryStorage.CsrfToken);
                })
                .Build();

            return connection;
        }

        private void InitializeViews()
        {

        }
    }
}
