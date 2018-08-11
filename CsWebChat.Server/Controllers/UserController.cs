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

            if (this.db.User.Any(x => x.Name.Equals(user.Name)))
            {
                result = Conflict(new { user.Name });
            }
            else
                try
                {
                    this.db.User.Add(user);
                    await this.db.SaveChangesAsync();

                    result = CreatedAtRoute("GetUserById", new { user.Id }, user);
                }
                catch
                {
                    result = BadRequest();
                }

            return result;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = nameof(GetUserById))]
        public async Task<ActionResult<User>> GetUserById(long id)
        {
            ActionResult result;

            if (id < 0)
            {
                result = BadRequest();
            }
            else
            {
                var user = await this.db.User.FindAsync(id);
                
                if(user == null)
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

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(long id, [FromBody] User user)
        {
            if (id < 0 || id != user.Id)
            {
                return BadRequest();
            }
            else
            {
                var userDb = await this.db.User.FindAsync(id);
                userDb.Name = user.Name;
                userDb.Password = user.Password;

                await this.db.SaveChangesAsync();
                return Ok();
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            ActionResult result = null;

            if (id < 0)
            {
                result = BadRequest();
            }
            else
            {
                try
                {
                    var user = await this.db.User.FindAsync(id);
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
