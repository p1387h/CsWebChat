using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.Models
{
    public class User
    {
        [Required]
        [MinLength(4)]
        [MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9]{4,20}$")]
        public string Name { get; set; }
        // No RegEx since the password should get hashed! Any limitations 
        // (i.e. characters) must be set in the client application.
        [Required]
        public string Password { get; set; }

        public IList<Message> MessageSent { get; set; }
        public IList<Message> MessageReceived { get; set; }
    }
}
