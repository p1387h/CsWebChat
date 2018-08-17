using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.SharedWebLogic.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Services
{
    class AuthenticationService
    {
        private readonly AntiforgeryStorage _storage;
        private readonly AntiforgeryService _antiforgeryService;
        // The container is used instead of a reference to the chat request since
        // these references are disposed of after a reuqest has been made.
        private readonly IUnityContainer _container;

        public AuthenticationService(AntiforgeryStorage storage, AntiforgeryService antiforgeryService, IUnityContainer container)
        {
            if (storage == null || antiforgeryService == null || container == null)
                throw new ArgumentException();

            this._storage = storage;
            this._antiforgeryService = antiforgeryService;
            this._container = container;
        }

        public async Task RegisterUser(User user, string address)
        {
            await this._antiforgeryService.TryRequestingAntiforgeryCookieTokenPair(address);



        }

        public async Task LoginUser(User user, string address)
        {
            await this._antiforgeryService.TryRequestingAntiforgeryCookieTokenPair(address);



        }
    }
}
