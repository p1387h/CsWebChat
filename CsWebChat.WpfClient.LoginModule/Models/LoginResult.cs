using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Models
{
    class LoginResult
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public LoginResponse Response { get; set; }
    }
}
