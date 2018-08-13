using CsWebChat.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.AuthorizationAttributes
{
    public class MessageAvailabilityHandler : AuthorizationHandler<MessageAvailabilityRequirement, Tuple<User, User>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MessageAvailabilityRequirement requirement, Tuple<User, User> resource)
        {
            var validUserNameOne = resource.Item1.Name;
            var validUserNameTwo = resource.Item2.Name;
            var userName = context.User.Identity.Name;

            // Username must match either the sender name or the receiver name
            // of the message itself. This prevents others from reading personal 
            // messages.
            if (!context.User.Identity.IsAuthenticated || context.User.Identity.Name == null
                || (!userName.Equals(validUserNameOne) && !userName.Equals(validUserNameTwo)))
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
