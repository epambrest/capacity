using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageMemberWorkingDaysService
    {
        Task<MemberWorkingDaysBusiness> GetWorkingDaysByIdAsync(int workingDaysId);
        Task<IEnumerable<MemberWorkingDaysBusiness>> GetAllWorkingDaysForSprintAsync(int sprintId);
        Task<bool> EditMemberWorkingDaysAsync(MemberWorkingDaysBusiness memberWorkingDays);
        Task<bool> AddMemberWorkingDaysAsync(MemberWorkingDaysBusiness memberWorkingDays);
    }
}
