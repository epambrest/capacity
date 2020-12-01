using Microsoft.AspNetCore.Identity;

namespace Teams.Web.ViewModels.TeamMember
{
    public class TeamMemberViewModel
    {
        public int TeamId { get; set; }
        public string MemberId { get; set; }
        public virtual IdentityUser Member { get; set; }
    }
}
