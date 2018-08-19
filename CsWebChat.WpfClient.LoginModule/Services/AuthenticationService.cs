using CsWebChat.WpfClient.LoginModule.Models;
using CsWebChat.WpfClient.SharedWebLogic.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Services
{
    class AuthenticationService
    {
        private readonly AntiforgeryStorage _storage;
        private readonly AntiforgeryService _antiforgeryService;
        private readonly AddressStorage _addressStorage;
        // The container is used instead of a reference to the chat request since
        // these references are disposed of after a reuqest has been made.
        private readonly IUnityContainer _container;

        public AuthenticationService(AntiforgeryStorage storage, AntiforgeryService antiforgeryService,
            AddressStorage addressStorage, IUnityContainer container)
        {
            if (storage == null || antiforgeryService == null
                || addressStorage == null || container == null)
                throw new ArgumentException();

            this._storage = storage;
            this._antiforgeryService = antiforgeryService;
            this._container = container;
            this._addressStorage = addressStorage;
        }

        public async Task<RegisterResult> RegisterUser(User user)
        {
            await this._antiforgeryService.TryRequestingAntiforgeryCookieTokenPair();

            using (var request = this._container.Resolve<WsChatRequest>())
            {
                // The address of the server could may already end with a "/". In this case
                // another one is not needed.
                var combiner = (this._addressStorage.ServerAddress.EndsWith("/")) ? "" : "/";
                var address = String.Join(combiner, this._addressStorage.ServerAddress, "api/user");
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await request.Client.PostAsync(address, content);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return new RegisterResult() { Success = true };
                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    return new RegisterResult()
                    {
                        Success = false,
                        StatusCode = response.StatusCode,
                        Response = JsonConvert.DeserializeObject<RegisterResponse>(jsonResponse)
                    };
                }
            }
        }

        public async Task<LoginResult> LoginUser(User user)
        {
            await this._antiforgeryService.TryRequestingAntiforgeryCookieTokenPair();

            using (var request = this._container.Resolve<WsChatRequest>())
            {
                // The address of the server could may already end with a "/". In this case
                // another one is not needed.
                var combiner = (this._addressStorage.ServerAddress.EndsWith("/")) ? "" : "/";
                var address = String.Join(combiner, this._addressStorage.ServerAddress, "api/authentication/login");
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await request.Client.PostAsync(address, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new LoginResult() { Success = true };
                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    return new LoginResult()
                    {
                        Success = false,
                        StatusCode = response.StatusCode,
                        Response = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse)
                    };
                }
            }
        }
    }
}
