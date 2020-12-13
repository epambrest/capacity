using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Business.Services
{
    public interface IManageMemberWorkingDaysService
    {
        Task<MemberWorkingDays> GetWorkingDaysByIdAsync(int workingDaysId);
        Task<IEnumerable<MemberWorkingDays>> GetAllWorkingDaysForSprintAsync(int sprintId);
        Task<bool> EditMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays);
        Task<bool> AddMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays);
    }
}
