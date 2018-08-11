using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace CsWebChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ChatContext db;

        public AuthenticationController(ChatContext db)
        {
            if (db == null)
                throw new ArgumentException();

            this.db = db;
        }

        // GET: api/Authentication/Denied
        [HttpGet("denied")]
        public void Denied()
        {
            HttpContext.Response.StatusCode = 401;
        }

        // POST: api/Authentication/Login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            var isInDb = this.db.User
                .Where(x => x.Name.Equals(user.Name) && x.Password.Equals(user.Password))
                .SingleOrDefault() != null;

            // An invalid combination must not authenticate the user.
            if(!isInDb)
            {
                return BadRequest();
            }
            else
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "User")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var properties = new AuthenticationProperties()
                {
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    properties);

                return new EmptyResult();
            }
        }

        // POST: api/Authentication/Logout
        [HttpPost("logout")]
        [Authorize(Policy = "LogoutPolicy")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new EmptyResult();
        }
    }
}