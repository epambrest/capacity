using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageMemberWorkingDaysService
    {
        Task<MemberWorkingDays> GetWorkingDaysByIdAsync(int workingDaysId);
        Task<IEnumerable<MemberWorkingDays>> GetAllWorkingDaysForSprintAsync(int sprintId);
        Task<bool> EditMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays);
        Task<bool> AddMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays);
        Task<double> GetStoryPointsInDayForMember(int sprintId, int teamMemberId, int teamMemberTotalSp);
    }
}
