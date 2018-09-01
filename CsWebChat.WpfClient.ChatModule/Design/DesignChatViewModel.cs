using CsWebChat.WpfClient.ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.ChatModule.Design
{
    class DesignUserViewModel
    {
        public ObservableCollection<User> Users { get; set; }

        public DesignUserViewModel()
        {
            Users = new ObservableCollection<User>();
            Users.AddRange(new User[] {
                new User() { Name = "TestUserOne" },
                new User() { Name = "TestUserTwo" }
            });
        }
    }
}
