using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.DAL
{
    public class Message
    {
        public long MessageId { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public DateTime TimeSent { get; set; }
        public string Content { get; set; }

        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }
}
