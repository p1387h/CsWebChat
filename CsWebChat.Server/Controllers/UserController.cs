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
        // POST: api/User
        [HttpPost]
        public ActionResult Register([FromBody] User user)
        {
            user.Id = 1;

            return CreatedAtRoute("GetUserById", new { user.Id }, user);
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = nameof(GetUserById))]
        public ActionResult<string> GetUserById(int id)
        {
            return "value";
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
