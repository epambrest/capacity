using Microsoft.AspNetCore.Identity;

namespace Teams.Business.Models
{
    public class UserBusiness : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
