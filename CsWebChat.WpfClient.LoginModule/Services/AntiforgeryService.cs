using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.SharedWebLogic.Models;
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
        private readonly AntiforgeryStorage _storage;
        // The container is used instead of a reference to the chat request since
        // these references are disposed of after a reuqest has been made.
        private readonly IUnityContainer _container;

        public AntiforgeryService(AntiforgeryStorage storage, IUnityContainer container)
        {
            if (storage == null || container == null)
                throw new ArgumentException();

            this._storage = storage;
            this._container = container;
        }

        public async Task TryRequestingAntiforgeryCookieTokenPair(string address)
        {
            if(this._storage.CsrfHeader == null || this._storage.CsrfToken == null)
            {
                using (var request = this._container.Resolve<WsChatRequest>())
                {
                    var result = await request.Client.GetAsync(address);
                    var jsonResponse = await result.Content.ReadAsStringAsync();
                    var csrfReponse = JsonConvert.DeserializeObject<CsrfResponse>(jsonResponse);
                    var cookie = request.Container.GetCookies(new Uri(address))["AntiforgeryCookie"];

                    // The combination of token and cookie must be set in order to 
                    // communicate with the server.
                    this._storage.CsrfHeader = csrfReponse.HeaderName;
                    this._storage.CsrfToken = csrfReponse.RequestToken;
                    this._storage.AntiforgeryCookieName = cookie.Name;
                    this._storage.AntiforgeryCookieContent = cookie.Value;
                }
            }
        }
    }
}
