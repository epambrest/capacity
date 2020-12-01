using Microsoft.AspNetCore.Identity;

namespace Teams.Web.ViewModels.Team
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public virtual IdentityUser Owner { get; set; }
    }
}
