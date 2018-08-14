using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.Server.Ws.DTOs
{
    public class Message
    {
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
