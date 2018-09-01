using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.Server.Ws
{
    public class ChatHubStorage
    {
        // Mapping for directly addressing a specific user based on his name.
        // Key:     Username
        // Value:   ConnectionId
        public Dictionary<string, string> MappedConnectionIds { get; private set; }

        public ChatHubStorage()
        {
            MappedConnectionIds = new Dictionary<string, string>();
        }
    }
}
