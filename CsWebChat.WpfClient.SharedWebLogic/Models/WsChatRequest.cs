﻿using System;
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

        private readonly AntiforgeryStorage _storage;

        public WsChatRequest(AntiforgeryStorage storage)
        {
            if (storage == null)
                throw new ArgumentException();

            this._storage = storage;
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
            if (!String.IsNullOrEmpty(storage.AntiforgeryCookieName))
            {
                var cookie = new Cookie(storage.AntiforgeryCookieName, storage.AntiforgeryCookieContent);
                Container.Add(cookie);
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
