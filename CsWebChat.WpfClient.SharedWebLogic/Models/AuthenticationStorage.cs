using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.WpfClient.SharedWebLogic.Models
{
    public class AuthenticationStorage : IDisposable
    {
        public string AuthenticationCookieName { get; set; }
        public string AuthenticationCookieContent { get; set; }

        // Should be called when the user presses the logout button.
        public void Dispose()
        {
            AuthenticationCookieName = null;
            AuthenticationCookieContent = null;
        }
    }
}
