using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.Models
{
    public class Message
    {
        public long MessageId { get; set; }
        [Required]
        public User Sender { get; set; }
        [Required]
        public User Receiver { get; set; }
        [Required]
        public DateTime TimeSent { get; set; }
    }
}
