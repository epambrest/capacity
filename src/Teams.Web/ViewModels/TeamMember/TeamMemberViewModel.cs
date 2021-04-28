using Teams.Business.Models;

namespace Teams.Web.ViewModels.TeamMember
{
    public class TeamMemberViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string MemberId { get; set; }
        public virtual User Member { get; set; }

        private TeamMemberViewModel(Business.Models.TeamMember teamMember)
        {
            Id = teamMember.Id;
            Member = teamMember.Member;
            MemberId = teamMember.MemberId;

            if (teamMember.Member != null)
            {
                Member = teamMember.Member;
            }

        }

        public static TeamMemberViewModel Create(Business.Models.TeamMember teamMember)
        {
            if (teamMember == null) return null;
            return new TeamMemberViewModel(teamMember);
        }
    }
}
