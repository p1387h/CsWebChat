using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.SharedWebLogic.Models
{
    public class WsChatRequest : IDisposable
    {
        public HttpClient Client { get; private set; }
        public CookieContainer Container { get; private set; }
        private readonly HttpClientHandler _handler;

        private readonly AntiforgeryStorage _antiforgeryStorage;
        private readonly AuthenticationStorage _authenticationStorage;
        private readonly AddressStorage _addressStorage;

        public WsChatRequest(AntiforgeryStorage antiforgeryStorage, AuthenticationStorage authenticationStorage,
            AddressStorage addressStorage)
        {
            if (antiforgeryStorage == null || authenticationStorage == null
                || addressStorage == null)
                throw new ArgumentException();

            this._antiforgeryStorage = antiforgeryStorage;
            this._authenticationStorage = authenticationStorage;
            this._addressStorage = addressStorage;

            // Domain has to be set in order to prevent ArgurmentExceptions when adding
            // the Cookie to the CookieContainer.
            var target = new Uri(addressStorage.ServerAddress);
            var domain = target.Host;
            Container = new CookieContainer();

            // Initialize the HttpClient with all the provided information inside the 
            // AuthenticationStorage.
            this._handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = Container
            };
            Client = new HttpClient(this._handler);

            // Ensure that the provided information are forwarded toards the server if
            // present.
            if (!String.IsNullOrEmpty(antiforgeryStorage.AntiforgeryCookieName))
            {
                var cookie = new Cookie(antiforgeryStorage.AntiforgeryCookieName, 
                    antiforgeryStorage.AntiforgeryCookieContent)
                {
                    Domain = domain
                };
                Container.Add(cookie);
            }
            if (!String.IsNullOrEmpty(antiforgeryStorage.CsrfHeader))
            {
                Client.DefaultRequestHeaders.Add(antiforgeryStorage.CsrfHeader, antiforgeryStorage.CsrfToken);
            }
            if (!String.IsNullOrEmpty(authenticationStorage.AuthenticationCookieName))
            {
                var cookie = new Cookie(authenticationStorage.AuthenticationCookieName,
                    authenticationStorage.AuthenticationCookieContent)
                {
                    Domain = domain
                };
                Container.Add(cookie);
            }
        }

        public void Dispose()
        {
            Client.Dispose();
            this._handler.Dispose();
        }
    }
}
