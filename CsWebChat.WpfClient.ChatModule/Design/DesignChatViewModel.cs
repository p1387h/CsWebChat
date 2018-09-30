using CsWebChat.WpfClient.ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.Design
{
    class DesignChatViewModel
    {
        public ObservableCollection<Message> Messages { get; set; }
        public string ChatPartnerName { get; private set; }

        private string _ownName = "User";

        public DesignChatViewModel()
        {
            this.ChatPartnerName = "Testpartner";

            this.Messages = new ObservableCollection<Message>()
            {
                new Message()
                {
                    Content = "Testmessage1",
                    MessageId = 1,
                    TimeSent = DateTime.MinValue,
                    Sender = new User()
                    {
                        Name = this.ChatPartnerName
                    },
                    Receiver = new User()
                    {
                        Name = this._ownName
                    }
                },
                new Message()
                {
                    Content = "Testmessage2",
                    MessageId = 2,
                    TimeSent = DateTime.MinValue,
                    Sender = new User()
                    {
                        Name = this._ownName
                    },
                    Receiver = new User()
                    {
                        Name = this.ChatPartnerName
                    }
                }
            };
        }
    }
}
