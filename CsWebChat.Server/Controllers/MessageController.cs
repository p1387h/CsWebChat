using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ChatContext _db;
        private readonly IAuthorizationService _authorizationService;

        public MessageController(ChatContext db, IAuthorizationService authorizationService)
        {
            if (db == null || authorizationService == null)
                throw new ArgumentException();

            this._db = db;
            this._authorizationService = authorizationService;
        }

        // GET: api/Message/5
        [HttpGet("{id:long}")]
        [Authorize(Policy = "LoggedInPolicy")]
        public async Task<ActionResult<Message>> GetMessageById(long id)
        {
            ActionResult result;
            var messsage = await this._db.Message.FindAsync(id);
            var validUsers = new Tuple<User, User>(messsage.Sender, messsage.Receiver);
            var maySeeMessage = await this._authorizationService.AuthorizeAsync(
                HttpContext.User,
                validUsers,
                new MessageAvailabilityRequirement());

            // Only allow either a receiver or a sender to view their own messages.
            if (maySeeMessage.Succeeded)
            {
                result = Ok(messsage);
            }
            else
            {
                result = Forbid();
            }

            return result;
        }
    }
}