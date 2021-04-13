using Teams.Business.Models;

namespace Teams.Web.ViewModels.TeamMember
{
    public class TeamMemberViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string MemberId { get; set; }
        public virtual User Member { get; set; }
    }
}
