using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using System.Security.Claims;

namespace Teams.Security
{
    public class CurrentUser : ICurrentUser
    {
        IHttpContextAccessor _accessor;
        public CurrentUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Id()
        {
            if (_accessor != null && _accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                throw new AuthenticationException("authentication error");
            }
        }

        public string Name()
        {
            if (_accessor != null && _accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return _accessor.HttpContext.User.Identity.Name;
            }
            else
            {
                throw new AuthenticationException("authentication error");
            }
        }
    }
}
