using CsWebChat.WpfClient.WebLogicModule.Models;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfCLient.WebLogicModule
{
    public class WebLogicModule : IModule
    {
        private readonly IUnityContainer _container;

        public WebLogicModule(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentException();

            this._container = container;
        }

        public void Initialize()
        {
            this.InitializeServices();
            this.InitializeViews();
        }

        public void InitializeServices()
        {
            var addressStorage = new AddressStorage();
            // Default server address for testing locally.
            addressStorage.Servers.Add("http://localhost:5000/");

            this._container.RegisterInstance<AuthenticationStorage>(new AuthenticationStorage());
            this._container.RegisterInstance<AntiforgeryStorage>(new AntiforgeryStorage());
            this._container.RegisterInstance<AddressStorage>(addressStorage);

            this._container.RegisterType<WsChatRequest>();
        }

        public void InitializeViews()
        {

        }
    }
}
