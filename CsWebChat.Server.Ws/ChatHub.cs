using AutoMapper;
using CsWebChat.Server.Ws.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsWebChat.Server.Ws
{
    [Authorize(Policy = "LoggedInPolicy")]
    public class ChatHub : Hub<IClient>
    {
        // Mapping for directly addressing a specific user based on his name.
        // Key:     Username
        // Value:   ConnectionId
        private readonly Dictionary<string, string> _mappedConnectionIds = new Dictionary<string, string>();
        private readonly IMapper _mapper;
        private readonly DAL.ChatContext _db;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(DAL.ChatContext db, ILogger<ChatHub> logger)
        {
            if (db == null || logger == null)
                throw new ArgumentException();

            this._db = db;
            this._logger = logger;

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

            this._mappedConnectionIds.Add(name, id);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;

            this._mappedConnectionIds.Remove(name);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageTo(string userName, string content)
        {
            try
            {
                var receiverId = this._mappedConnectionIds[userName];
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
    }
}
