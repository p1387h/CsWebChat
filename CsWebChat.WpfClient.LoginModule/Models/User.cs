using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Models
{
    class User
    {
        public string UserName { get; set; }
        public SecureString Password { get; set; }
    }
}
