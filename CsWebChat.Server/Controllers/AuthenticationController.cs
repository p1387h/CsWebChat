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
using Microsoft.AspNetCore.Antiforgery;

namespace CsWebChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ChatContext _db;
        private readonly IAntiforgery _antiforgery;

        public AuthenticationController(ChatContext db, IAntiforgery antiforgery)
        {
            if (db == null || antiforgery == null)
                throw new ArgumentException();

            this._db = db;
            this._antiforgery = antiforgery;
        }

        // GET: api/Authentication/Denied
        [HttpGet("denied")]
        public void Denied()
        {
            HttpContext.Response.StatusCode = 401;
        }

        // GET api/Authentication/csrftoken
        [HttpGet("csrftoken")]
        public ActionResult CsrfToken()
        {
            var tokens = this._antiforgery.GetAndStoreTokens(HttpContext);

            return new ObjectResult(new
            {
                headerName = tokens.HeaderName,
                requestToken = tokens.RequestToken
            });
        }

        // POST: api/Authentication/Login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            var isInDb = this._db.User
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
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "LogoutPolicy")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new EmptyResult();
        }
    }
}