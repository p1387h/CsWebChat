using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.Events
{
    class WebsocketConnectionStateEvent : PubSubEvent<WebSocketState>
    {
    }
}
