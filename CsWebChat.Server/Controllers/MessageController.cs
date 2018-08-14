using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsWebChat.Server.AuthorizationAttributes;
using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CsWebChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly DAL.ChatContext _db;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public MessageController(DAL.ChatContext db, IAuthorizationService authorizationService, IMapper mapper)
        {
            if (db == null || authorizationService == null || mapper == null)
                throw new ArgumentException();

            this._db = db;
            this._authorizationService = authorizationService;
            this._mapper = mapper;
        }

        // GET: api/Message/5
        [HttpGet("{id:long}")]
        [Authorize(Policy = "LoggedInPolicy")]
        public async Task<ActionResult<Message>> GetMessageById(long id)
        {
            ActionResult result;
            var messageDb = await this._db.Message.FindAsync(id);
            var message = this._mapper.Map<Message>(messageDb);
            var validUsers = new Tuple<User, User>(message.Sender, message.Receiver);
            var maySeeMessage = await this._authorizationService.AuthorizeAsync(
                HttpContext.User,
                validUsers,
                new MessageAvailabilityRequirement());

            // Only allow either a receiver or a sender to view their own messages.
            if (maySeeMessage.Succeeded)
            {
                result = Ok(message);
            }
            else
            {
                result = Forbid();
            }

            return result;
        }

        // GET: api/Message
        [HttpGet]
        [Authorize(Policy = "LoggedInPolicy")]
        public ActionResult<List<Message>> GetOwnMessages([FromQuery] string restriction)
        {
            var onlySent = restriction?.Equals("sent") ?? false;
            var onlyReceived = restriction?.Equals("received") ?? false;
            var name = HttpContext.User.Identity.Name;
            List<DAL.Message> messagesDb = null;
            List<Message> messages = null;

            if(onlySent)
            {
                messagesDb = this._db.Message
                    .Where(x => x.SenderName.Equals(name))
                    .ToList();
            }
            else if(onlyReceived)
            {
                messagesDb = this._db.Message
                    .Where(x => x.ReceiverName.Equals(name))
                    .ToList();
            }
            else
            {
                messagesDb = this._db.Message
                    .Where(x => x.SenderName.Equals(name) || x.ReceiverName.Equals(name))
                    .ToList();
            }

            messages = this._mapper.Map<List<DAL.Message>, List<Message>>(messagesDb);

            return Ok(messages);
        }
    }
}