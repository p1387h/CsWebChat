using CsWebChat.Server.Ws.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsWebChat.Server.Ws
{
    [Authorize(Policy = "LoggedInPolicy")]
    public class ChatHub : Hub<IClient>
    {
        // Mapping for directly addressing a specific user based on his name.
        // Key:     Username
        // Value:   ConnectionId
        private readonly Dictionary<string, string> _mappedConnectionIds = new Dictionary<string, string>();
        private ILogger _logger;

        public ChatHub(ILogger logger)
        {
            this._logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var id = Context.ConnectionId;

            this._mappedConnectionIds.Add(name, id);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;

            this._mappedConnectionIds.Remove(name);

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendMessageTo(string userName, Message message)
        {
            try
            {
                var id = this._mappedConnectionIds[userName];
                var client = Clients.Client(id);
                var senderName = Context.User.Identity.Name;
                var sender = new User() { Name = senderName };

                client?.ReceiveMessage(sender, message);
            } catch(KeyNotFoundException e)
            {
                this._logger.LogError(e, String.Format("User '{0}' not found.", userName));
            }

            return Task.CompletedTask;
        }
    }
}
