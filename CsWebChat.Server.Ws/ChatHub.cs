using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
