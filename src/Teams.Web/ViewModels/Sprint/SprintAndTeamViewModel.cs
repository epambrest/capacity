using System.Collections.Generic;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintAndTeamViewModel
    {
        public List<SprintViewModel> Sprints { get; set; }
        public TeamViewModel Team { get; set; }
    }
}
