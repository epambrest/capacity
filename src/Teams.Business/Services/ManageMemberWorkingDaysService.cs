using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

namespace Teams.Business.Services
{
    public class ManageMemberWorkingDaysService : IManageMemberWorkingDaysService
    {
        private readonly IRepository<MemberWorkingDays, int> _memberWorkingDaysRepository;
        private readonly ICurrentUser _currentUser;

        public ManageMemberWorkingDaysService(IRepository<MemberWorkingDays, int> memberWorkingDaysRepository, ICurrentUser currentUser)
        {
            _memberWorkingDaysRepository = memberWorkingDaysRepository;
            _currentUser = currentUser;
        }

        public async Task<MemberWorkingDays> GetWorkingDaysByIdAsync(int workingDaysId)
        {
            var workingDays = await _memberWorkingDaysRepository.GetAll()
                .Include(x => x.Sprint)
                .Include(x => x.TeamMember)
                .FirstOrDefaultAsync(x => x.Id == workingDaysId);
            return workingDays;
        }

        public async Task<bool> AddMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays)
        {
            if (memberWorkingDays.WorkingDays < 0)
            {
                return false;
            }
            return await _memberWorkingDaysRepository.InsertAsync(memberWorkingDays);
        }

        public async Task<bool> EditMemberWorkingDaysAsync(MemberWorkingDays memberWorkingDays)
        {
            var oldMemberWorkingDays = await _memberWorkingDaysRepository.GetByIdAsync(memberWorkingDays.Id);

            if (oldMemberWorkingDays == null ||
                memberWorkingDays.WorkingDays < 0)
            {
                return false;
            }

            return await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);
        }

        public async Task<IEnumerable<MemberWorkingDays>> GetAllWorkingDaysForSprintAsync(int sprintId)
        {
            var workingDays = await _memberWorkingDaysRepository.GetAll()
                .Include(x => x.Sprint)
                .Include(x => x.TeamMember)
                .Where(x => x.SprintId == sprintId).ToListAsync();
            return workingDays;
        }
    }
}
