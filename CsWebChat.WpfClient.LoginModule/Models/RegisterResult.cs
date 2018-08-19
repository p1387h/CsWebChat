using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Models
{
    class RegisterResult
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode{ get; set; }
        public RegisterResponse Response { get; set; }
    }
}
