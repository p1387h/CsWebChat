using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.WpfClient.LoginModule.Models
{
    class CsrfResponse
    {
        public string HeaderName { get; set; }
        public string RequestToken { get; set; }
    }
}
