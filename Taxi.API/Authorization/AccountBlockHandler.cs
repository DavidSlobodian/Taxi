using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Taxi.App.Services.AuthService;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure.Repos;

namespace Taxi.API.Authorization
{
    public class AccountBlockHandler : AuthorizationHandler<AccountBlockRequirement>
    {
        private readonly IGenericRepository<User> _userRepos;
        public AccountBlockHandler(IGenericRepository<User> userRepos)
        {
            _userRepos = userRepos;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountBlockRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                //return;
                throw new NotFoundException("UserId couldn't be found");
            }
            var user = await _userRepos.GetById(Guid.Parse(userId.Value));
            if (user == null)
            {
                //return;
                throw new NotFoundException("User couldn't be found");
            }
            if (user.AccountStatus == Domain.Enums.AccountStatus.Blocked)
            {
                //return;
                throw new ForbiddenException("The account is blocked");
            }
            context.Succeed(requirement);
        }
    }
}
