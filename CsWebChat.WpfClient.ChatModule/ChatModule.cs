using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Views registered for the possibility of INavigationAware returning false and
            // the RegionManager therefore requiring a new instance. The name is needed since
            // a Uri is used for resolving the type when navigating.
            //this._container.RegisterType<object, T>(nameof(T));
        }

        private void InitializeViews()
        {

        }
    }
}
