using System.Collections.Generic;
using Teams.Web.ViewModels.Team;
using Teams.Web.ViewModels.MemberWorkingDays;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintAndTeamViewModel
    {
        public List<SprintViewModel> Sprints { get; set; }
        public TeamViewModel Team { get; set; }
        public List<MemberWorkingDaysViewModels> memberWorkingDays { get; set; }
        public int sprintId { get; set; }
    }
}
