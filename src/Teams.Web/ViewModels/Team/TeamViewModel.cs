using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Team
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public virtual IdentityUser Owner { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
    }
}
