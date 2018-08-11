using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CsWebChat.Server.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IApplicationLifetime _applicationLifetime;

        public StatusController(IApplicationLifetime applicationLifetime)
        {
            if (applicationLifetime == null)
                throw new ArgumentException();

            this._applicationLifetime = applicationLifetime;
        }

        // An easy way to completely stop the kestrel server. Should only be accessible
        // by the admin!
        // POST: api/Status/Shutdown
        [HttpPost("shutdown")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult ShutdownServer()
        {
            this._applicationLifetime.StopApplication();
            return Ok("Server stopped.");
        }
    }
}