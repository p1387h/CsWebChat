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

            if(!this._chatHubStorage.MappedConnectionIds.ContainsKey(name))
            {
                this._chatHubStorage.MappedConnectionIds[name] = new List<string>();
            }
            this._chatHubStorage.MappedConnectionIds[name].Add(id);

            // Make sure only to notify users with different names!
            this.Clients
                .AllExcept(this._chatHubStorage.MappedConnectionIds[name])
                .NotifyUsersStateChangesAsync(new List<string>() { name }, UserState.Online);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;
            var id = Context.ConnectionId;

            this._chatHubStorage.MappedConnectionIds[name].Remove(id);

            if(!this._chatHubStorage.MappedConnectionIds[name].Any())
            {
                this._chatHubStorage.MappedConnectionIds.Remove(name);
                // Others can be used since it was the last connection of the
                // user.
                this.Clients.Others.NotifyUsersStateChangesAsync(new List<string>() { name }, UserState.Offline);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageTo(string userName, string content)
        {
            try
            {
                var receiver = new User() { Name = userName };
                var receiverIds = this._chatHubStorage.MappedConnectionIds[userName];
                var senderName = Context.User.Identity.Name;
                var sender = new User() { Name = senderName };
                var message = new Message()
                {
                    Receiver = receiver,
                    Sender = sender,
                    TimeSent = DateTime.Now,
                    Content = content
                };

                foreach (var receiverId in receiverIds)
                {
                    var receiverClient = Clients.Client(receiverId);

                    if (receiverClient != null)
                    {
                        await receiverClient.ReceiveMessageAsync(message);
                    }
                }

                var messageDb = this._mapper.Map<DAL.Message>(message);
                this._db.Message.Add(messageDb);
                await this._db.SaveChangesAsync();
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
