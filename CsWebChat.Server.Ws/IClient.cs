using CsWebChat.Server.Ws.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.Server.Ws
{
    public interface IClient
    {
        void ReceiveMessage(User sender, Message message);
    }
}
