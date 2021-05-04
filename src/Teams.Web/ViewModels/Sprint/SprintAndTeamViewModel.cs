using System.Collections.Generic;
using Teams.Web.ViewModels.MemberWorkingDays;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintAndTeamViewModel
    {
        public List<SprintViewModel> Sprints { get; set; }
        public TeamViewModel Team { get; set; }
        public List<MemberWorkingDaysViewModel> memberWorkingDays { get; set; }
        public int sprintId { get; set; }
        public int DaysInSprint { get; set; }
        public bool IsOwner { get; set; }

        private SprintAndTeamViewModel(Business.Models.Sprint currentSprint, 
            List<SprintViewModel> sprintViewModels,
            TeamViewModel teamViewModel, 
            List<MemberWorkingDaysViewModel> memberWorkingDaysViewModel)
        {
            if (currentSprint != null)
            {
                sprintId = currentSprint.Id;
                DaysInSprint = currentSprint.DaysInSprint;
            }

            IsOwner = teamViewModel.IsOwner;
            Sprints = sprintViewModels;
            Team = teamViewModel;
            memberWorkingDays = memberWorkingDaysViewModel;
        }

        public static SprintAndTeamViewModel Create(Business.Models.Sprint currentSprint, 
            List<Business.Models.Sprint> sprints,
            Business.Models.Team team, 
            bool isOwner, 
            List<Business.Models.MemberWorkingDays> memberWorkingDays)
        {
            var sprintViewModels = new List<SprintViewModel>();
            var teamViewModel = TeamViewModel.Create(team, isOwner, new List<Business.Models.TeamMember>());
            var memberWorkingDaysViewModels = new List<MemberWorkingDaysViewModel>();

            foreach (var sprint in sprints)
            {
                var sprintViewModel = SprintViewModel.Create(sprint, isOwner, 0);
                sprintViewModels.Add(sprintViewModel);
            }

            foreach (var memberWorkingDay in memberWorkingDays)
            {
                var memberWorkingDayViewModel = MemberWorkingDaysViewModel.Create(memberWorkingDay);
                memberWorkingDaysViewModels.Add(memberWorkingDayViewModel);
            }

            return new SprintAndTeamViewModel(currentSprint, sprintViewModels, teamViewModel, memberWorkingDaysViewModels);
        }
    }
}