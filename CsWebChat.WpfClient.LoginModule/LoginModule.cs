using CsWebChat.WpfClient.LoginModule.Views;
using CsWebChat.WpfClient.Regions;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            
        }

        private void InitializeRegions()
        {
            this._regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(LoginView));
            this._regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(RegisterView));
        }
    }
}
