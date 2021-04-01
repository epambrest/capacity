using System.Collections.Generic;
using Teams.Web.ViewModels.MemberWorkingDays;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintAndTeamViewModel
    {
        public List<SprintViewModel> Sprints { get; set; }
        public TeamViewModel Team { get; set; }
        public List<MemberWorkingDaysViewModels> memberWorkingDays { get; set; }
        public int sprintId { get; set; }
        public int DaysInSprint { get; set; }
        public bool IsOwner { get; set; }
    }
}