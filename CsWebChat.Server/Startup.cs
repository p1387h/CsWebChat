using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace CsWebChat.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie((options) => 
            {
                options.Cookie.Name = "AuthenticationCookie";
                options.Cookie.MaxAge = TimeSpan.FromMinutes(10);
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                // Since a request to a [Authorize]d resource without the necessary permissions
                // causes the middleware to redirect to the login path, a workaround is needed
                // in order to allow 401 responses to reach the user.
                options.AccessDeniedPath = "/api/authentication/denied";
                options.LoginPath = "/api/authentication/denied";
                options.LogoutPath = "/api/authentication/logout";
                options.SlidingExpiration = true;
            });
            services.AddDbContext<ChatContext>((options) =>
            {
                options.UseInMemoryDatabase("ChatDatabase");
            });
            services.AddAuthorization((options) => 
            {
                options.AddPolicy("LogoutPolicy", (config) => 
                {
                    config.RequireRole("Admin", "User");
                });
            });
            services.AddAntiforgery((options) =>
            {
                options.Cookie.Name = "AntiforgeryCookie";
                options.HeaderName = "AntiforgeryHeader";
                options.FormFieldName = "AntiforgeryFormField";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
