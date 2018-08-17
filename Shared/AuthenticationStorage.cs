using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.WebLogic
{
    public class AuthenticationStorage
    {
        public string CsrfHeader { get; set; }
        public string CsrfToken { get; set; }
        public string AuthenticationCookieName { get; set; }
        public string AuthenticationCookieContent { get; set; }
        
        public void Clear()
        {
            CsrfHeader = null;
            CsrfToken = null;
            AuthenticationCookieName = null;
            AuthenticationCookieContent = null;
        }
    }
}
