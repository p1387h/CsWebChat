using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.WebLogicModule.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Services
{
    class AntiforgeryService
    {
        private readonly AntiforgeryStorage _antiforgeryStorage;
        private readonly AddressStorage _addressStorage;
        // The container is used instead of a reference to the chat request since
        // these references are disposed of after a reuqest has been made.
        private readonly IUnityContainer _container;

        public AntiforgeryService(AntiforgeryStorage antiforgeryStorage, AddressStorage addressStorage, IUnityContainer container)
        {
            if (antiforgeryStorage == null || addressStorage == null || container == null)
                throw new ArgumentException();

            this._antiforgeryStorage = antiforgeryStorage;
            this._addressStorage = addressStorage;
            this._container = container;
        }

        public async Task TryRequestingAntiforgeryCookieTokenPair()
        {
            if(this._antiforgeryStorage.CsrfHeader == null || this._antiforgeryStorage.CsrfToken == null)
            {
                using (var request = this._container.Resolve<WsChatRequest>())
                {
                    // The address of the server could may already end with a "/". In this case
                    // another one is not needed.
                    var combiner = (this._addressStorage.ServerAddress.EndsWith("/")) ? "" : "/";
                    var address = String.Join(combiner, this._addressStorage.ServerAddress, "api/authentication/csrftoken");

                    var response = await request.Client.GetAsync(address);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var csrfReponse = JsonConvert.DeserializeObject<CsrfResponse>(jsonResponse);
                    var cookie = request.Container.GetCookies(new Uri(address))["AntiforgeryCookie"];

                    // The combination of token and cookie must be set in order to 
                    // communicate with the server.
                    this._antiforgeryStorage.CsrfHeader = csrfReponse.HeaderName;
                    this._antiforgeryStorage.CsrfToken = csrfReponse.RequestToken;
                    this._antiforgeryStorage.AntiforgeryCookieName = cookie.Name;
                    this._antiforgeryStorage.AntiforgeryCookieContent = cookie.Value;
                }
            }
        }
    }
}
