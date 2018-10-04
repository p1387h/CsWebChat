using CsWebChat.WpfClient.ChatModule.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.Events
{
    class MessageReceivedEvent : PubSubEvent<Message>
    {
    }
}
