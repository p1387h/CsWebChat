using AutoMapper;
using CsWebChat.Server.Ws.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CsWebChat.Server.Ws
{
    [Authorize(Policy = "LoggedInPolicy")]
    public class ChatHub : Hub<IClient>
    {
        private readonly IMapper _mapper;

        private readonly DAL.ChatContext _db;
        private readonly ILogger<ChatHub> _logger;
        private readonly ChatHubStorage _chatHubStorage;

        public ChatHub(DAL.ChatContext db, ILogger<ChatHub> logger,
            ChatHubStorage chatHubStorage)
        {
            if (db == null || logger == null
                || chatHubStorage == null)
                throw new ArgumentException();

            this._db = db;
            this._logger = logger;
            this._chatHubStorage = chatHubStorage;

            // AutoMapper configuration:
            var mapperConfig = new MapperConfiguration((config) =>
            {
                config.CreateMap<User, DAL.User>();
                config.CreateMap<DAL.User, User>();
                config.CreateMap<Message, DAL.Message>();
                config.CreateMap<DAL.Message, Message>();
            });
            this._mapper = mapperConfig.CreateMapper();
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var id = Context.ConnectionId;

            this._chatHubStorage.MappedConnectionIds.Add(name, id);
            this.Clients.Others.NotifyUsersStateChangesAsync(new List<string>() { name }, UserState.Online);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;

            this._chatHubStorage.MappedConnectionIds.Remove(name);
            this.Clients.Others.NotifyUsersStateChangesAsync(new List<string>() { name }, UserState.Offline);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageTo(string userName, string content)
        {
            try
            {
                var receiverId = this._chatHubStorage.MappedConnectionIds[userName];
                var receiverClient = Clients.Client(receiverId);
                var receiver = new User() { Name = userName };

                var senderName = Context.User.Identity.Name;
                var sender = new User() { Name = senderName };

                if (receiverClient != null)
                {
                    var message = new Message()
                    {
                        Receiver = receiver,
                        Sender = sender,
                        TimeSent = DateTime.Now,
                        Content = content
                    };
                    var messageDb = this._mapper.Map<DAL.Message>(message);

                    await receiverClient.ReceiveMessageAsync(message);

                    this._db.Message.Add(messageDb);
                    await this._db.SaveChangesAsync();
                }
            }
            catch (KeyNotFoundException e)
            {
                this._logger.LogError(e, String.Format("User '{0}' not found.", userName));
            }
        }

        public async Task RequestOtherUserStates()
        {
            var name = Context.User.Identity.Name;
            var otherUserNames = this._chatHubStorage
                .MappedConnectionIds
                .Where(x => !x.Key.Equals(name))
                .Select(x => x.Key);

            await this.Clients.Caller.NotifyUsersStateChangesAsync(otherUserNames, UserState.Online);
        }
    }
}
