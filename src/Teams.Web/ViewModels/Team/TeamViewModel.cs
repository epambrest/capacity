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
        public virtual User Owner { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }

        private TeamViewModel(Business.Models.Team team, bool isOwner, List<TeamMemberViewModel> teamMemberViewModels)
        {
            if (team != null)
            {
                Id = team.Id;
                Owner = team.Owner;
                TeamName = team.TeamName;
            }

            IsOwner = isOwner;
            TeamMembers = teamMemberViewModels;
        }

        public TeamViewModel() 
        { 
        }

        public static TeamViewModel Create(Business.Models.Team team, bool isOwner, List<Business.Models.TeamMember> allTeamMembers)
        {
            var teamMemberViewModels = new List<TeamMemberViewModel>();

            if (allTeamMembers.Count > 0)
            {
                foreach (var teamMember in allTeamMembers)
                {
                    var newTeamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                    teamMemberViewModels.Add(newTeamMemberViewModel);
                }
            }
            else
            {
                foreach (var teamMember in team.TeamMembers)
                {
                    var newTeamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                    teamMemberViewModels.Add(newTeamMemberViewModel);
                }
            }

            return new TeamViewModel(team, isOwner, teamMemberViewModels);
        }
    }
}
