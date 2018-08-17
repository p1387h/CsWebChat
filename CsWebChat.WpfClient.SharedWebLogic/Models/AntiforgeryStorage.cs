using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.SharedWebLogic.Models
{
    public class AntiforgeryStorage
    {
        public string CsrfHeader { get; set; }
        public string CsrfToken { get; set; }
        public string AntiforgeryCookieName { get; set; }
        public string AntiforgeryCookieContent { get; set; }
        
        public void Clear()
        {
            CsrfHeader = null;
            CsrfToken = null;
            AntiforgeryCookieName = null;
            AntiforgeryCookieContent = null;
        }
    }
}
