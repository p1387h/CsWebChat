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
    [AutoValidateAntiforgeryToken]
    public class UserController : ControllerBase
    {
        private readonly DAL.ChatContext _db;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public UserController(DAL.ChatContext db, IAuthorizationService authorizationService, IMapper mapper)
        {
            if (db == null || authorizationService == null || mapper == null)
                throw new ArgumentException();

            this._db = db;
            this._authorizationService = authorizationService;
            this._mapper = mapper;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            ActionResult result = null;

            if (await this._db.User.FindAsync(user.Name) != null)
            {
                result = Conflict(new { user.Name });
            }
            else
            {
                try
                {
                    var dbUser = this._mapper.Map<DAL.User>(user);

                    this._db.User.Add(dbUser);
                    await this._db.SaveChangesAsync();

                    result = CreatedAtRoute(nameof(GetUserByName), new { user.Name }, user);
                }
                catch
                {
                    result = BadRequest();
                }
            }

            return result;
        }

        // GET: api/User/X
        [HttpGet("{name}", Name = nameof(GetUserByName))]
        [Authorize(Policy = "LoggedInPolicy")]
        public async Task<ActionResult<User>> GetUserByName(string name)
        {
            ActionResult result;

            if (String.IsNullOrEmpty(name))
            {
                result = BadRequest();
            }
            else
            {
                var userDb = await this._db.User.FindAsync(name);

                if (userDb == null)
                {
                    result = NotFound();
                }
                else
                {
                    var user = this._mapper.Map<User>(userDb);
                    var maySeeCompleteInfo = await this._authorizationService.AuthorizeAsync(
                        HttpContext.User,
                        user,
                        new UserNameRequirement());

                    // Only the user himself is allowed to see all iformation regarding his profile.
                    // For other users some information must be removed when requesting this information.
                    if (!maySeeCompleteInfo.Succeeded)
                    {
                        user.MessageReceived = null;
                        user.MessageSent = null;
                    }
                    // Never return the password to the user.
                    user.Password = null;

                    result = Ok(user);
                }
            }

            return result;
        }

        // PUT: api/User/X
        [HttpPut("{name}")]
        [Authorize(Policy = "LoggedInPolicy")]
        public async Task<ActionResult> Put(string name, [FromBody] User user)
        {
            ActionResult result;

            if (String.IsNullOrEmpty(name) || !name.Equals(user.Name))
            {
                result = BadRequest();
            }
            else
            {
                var userDb = await this._db.User.FindAsync(name);
                var mayChangeInfo = await this._authorizationService.AuthorizeAsync(
                        HttpContext.User,
                        user,
                        new UserNameRequirement());

                // Only the user himself is allowed to change his own information.
                if (mayChangeInfo.Succeeded)
                {
                    userDb.Name = user.Name;
                    userDb.Password = user.Password;

                    await this._db.SaveChangesAsync();
                    result = Ok();
                }
                else
                {
                    result = Forbid();
                }
            }

            return result;
        }

        // DELETE: api/User/X
        [HttpDelete("{name}")]
        [Authorize(Policy = "LoggedInPolicy")]
        public async Task<ActionResult> Delete(string name)
        {
            ActionResult result = null;

            if (String.IsNullOrEmpty(name))
            {
                result = BadRequest();
            }
            else
            {
                try
                {
                    var userDb = await this._db.User.FindAsync(name);
                    var user = this._mapper.Map<User>(userDb);
                    var mayChangeInfo = await this._authorizationService.AuthorizeAsync(
                        HttpContext.User,
                        user,
                        new UserNameRequirement());

                    // Only the user himself is allowed to delete his account.
                    if (mayChangeInfo.Succeeded)
                    {
                        this._db.User.Remove(userDb);
                        result = Ok();
                    }
                    else
                    {
                        result = Forbid();
                    }
                }
                catch
                {
                    result = BadRequest();
                }
            }

            return result;
        }
    }
}
