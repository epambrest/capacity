using Microsoft.AspNetCore.Http;
using System;

namespace Teams.Security
{
    public class CurrentUser : ICurrentUser
    {

        public UserDetails Current { get => current.Value;}

        private readonly Lazy<UserDetails> current;

        public CurrentUser(IHttpContextAccessor accessor)
        {
            current = new Lazy<UserDetails>(() => new UserDetails(accessor));
        }
    }
}
