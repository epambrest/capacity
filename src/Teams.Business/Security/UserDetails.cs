using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using System.Security.Claims;

namespace Teams.Business.Security
{
    public class UserDetails
    {
        private readonly IHttpContextAccessor _accessor;
        public UserDetails(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public virtual string Id()
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

        public virtual string Name()
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
