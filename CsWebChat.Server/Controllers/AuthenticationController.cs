using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CsWebChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        // POST: api/Authentication/Login
        [HttpPost("login")]
        public ActionResult Login()
        {
            return new EmptyResult();
        }

        // POST: api/Authentication/Logout
        [HttpPost("logout")]
        public ActionResult Logout()
        {
            return new EmptyResult();
        }
    }
}