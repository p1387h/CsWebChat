using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CsWebChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class UserController : ControllerBase
    {
        private readonly ChatContext db;

        public UserController(ChatContext db)
        {
            if (db == null)
                throw new ArgumentException();

            this.db = db;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            ActionResult result = null;

            if (await this.db.User.FindAsync(user.Name) != null)
            {
                result = Conflict(new { user.Name });
            }
            else
            {
                try
                {
                    this.db.User.Add(user);
                    await this.db.SaveChangesAsync();

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
        public async Task<ActionResult<User>> GetUserByName(string name)
        {
            ActionResult result;

            if (String.IsNullOrEmpty(name))
            {
                result = BadRequest();
            }
            else
            {
                var user = await this.db.User.FindAsync(name);

                if (user == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(user);
                }
            }

            return result;
        }

        // PUT: api/User/X
        [HttpPut("{name}")]
        public async Task<ActionResult> Put(string name, [FromBody] User user)
        {
            if (String.IsNullOrEmpty(name) || !name.Equals(user.Name))
            {
                return BadRequest();
            }
            else
            {
                var userDb = await this.db.User.FindAsync(name);
                userDb.Name = user.Name;
                userDb.Password = user.Password;

                await this.db.SaveChangesAsync();
                return Ok();
            }
        }

        // DELETE: api/User/X
        [HttpDelete("{name}")]
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
                    var user = await this.db.User.FindAsync(name);
                    this.db.User.Remove(user);

                    result = Ok();
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
