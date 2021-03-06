﻿using CsWebChat.WpfClient.LoginModule.Services;
using CsWebChat.WpfClient.LoginModule.Views;
using CsWebChat.WpfClient.Regions;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule
{
    public class LoginModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public LoginModule(IUnityContainer container, IRegionManager regionManager)
        {
            if (container == null || regionManager == null)
                throw new ArgumentException();

            this._container = container;
            this._regionManager = regionManager;
        }

        public void Initialize()
        {
            this.InitializeServices();
            this.InitializeRegions();
        }

        private void InitializeServices()
        {
            this._container.RegisterType<AntiforgeryService>();
            this._container.RegisterType<AuthenticationService>();

            // Views registered for the possibility of INavigationAware returning false and
            // the RegionManager therefore requiring a new instance. The name is needed since
            // a Uri is used for resolving the type when navigating.
            this._container.RegisterType<object, LoginView>(nameof(LoginView));
            this._container.RegisterType<object, RegisterView>(nameof(RegisterView));
            this._container.RegisterType<object, LoginTabView>(nameof(LoginTabView));
            this._container.RegisterType<object, ServerView>(nameof(ServerView));
        }

        private void InitializeRegions()
        {
            this._regionManager.RegisterViewWithRegion(MainWindowRegionNames.MAIN_REGION, typeof(LoginTabView));
            this._regionManager.RegisterViewWithRegion(LoginModuleRegionNames.TAB_REGION, typeof(LoginView));
            this._regionManager.RegisterViewWithRegion(LoginModuleRegionNames.TAB_REGION, typeof(RegisterView));
            this._regionManager.RegisterViewWithRegion(LoginModuleRegionNames.TAB_REGION, typeof(ServerView));
        }
    }
}
