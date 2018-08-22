using System;
using System.Collections.Generic;
using System.Text;

namespace CsWebChat.WpfClient.WebLogicModule.Models
{
    public class AddressStorage
    {
        public string ServerAddress { get; set; }
        public List<string> Servers { get; private set; }

        public AddressStorage()
        {
            Servers = new List<string>();
        }
    }
}
