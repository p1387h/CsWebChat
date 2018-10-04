using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.Models
{
    class Message
    {
        public long MessageId { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public DateTime TimeSent { get; set; }
        public string Content { get; set; }

        public bool IsFromChatPartner { get; set; }
    }
}
