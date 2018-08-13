using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.AuthorizationAttributes
{
    public class UserNameHandler : AuthorizationHandler<UserNameRequirement, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserNameRequirement requirement, User resource)
        {
            var name = context.User.Identity.Name;

            if (name == null || !name.Equals(resource.Name))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
