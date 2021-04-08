using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Teams.Business.Models;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Team
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = "TeamName")]
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public bool IsOwner { get; set; }
        public virtual UserBusiness Owner { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
    }
}
