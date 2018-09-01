using CsWebChat.Server.Ws.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.Server.Ws
{
    public interface IClient
    {
        Task ReceiveMessageAsync(Message message);
        Task NotifyUsersStateChangesAsync(IEnumerable<string> usernames, UserState state);
        Task PongAsync();
    }
}
