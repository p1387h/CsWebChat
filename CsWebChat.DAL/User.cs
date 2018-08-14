using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.DAL
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public IList<Message> MessageSent { get; set; }
        public IList<Message> MessageReceived { get; set; }
    }
}
