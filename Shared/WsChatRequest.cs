using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.WebLogic
{
    public class WsChatRequest : IDisposable
    {
        public HttpClient Client { get; private set; }
        private readonly CookieContainer _container = new CookieContainer();
        private readonly HttpClientHandler _handler;

        private readonly AuthenticationStorage _storage;

        public WsChatRequest(AuthenticationStorage storage)
        {
            if (storage == null)
                throw new ArgumentException();

            _storage = storage;

            // Initialize the HttpClient with all the provided information inside the 
            // AuthenticationStorage.
            this._handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = this._container
            };
            Client = new HttpClient(this._handler);

            // Ensure that the provided information are forwarded toards the server if
            // present.
            if (!String.IsNullOrEmpty(storage.AuthenticationCookieName))
            {
                var cookie = new Cookie(storage.AuthenticationCookieName, storage.AuthenticationCookieContent);
                this._container.Add(cookie);
            }
            if (!String.IsNullOrEmpty(storage.CsrfHeader))
            {
                Client.DefaultRequestHeaders.Add(storage.CsrfHeader, storage.CsrfToken);
            }
        }

        public void Dispose()
        {
            Client.Dispose();
            this._handler.Dispose();
        }
    }
}
