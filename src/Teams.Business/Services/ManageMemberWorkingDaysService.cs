using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;

namespace Teams.Business.Services
{
    public class ManageMemberWorkingDaysService : IManageMemberWorkingDaysService
    {
        private readonly IRepository<MemberWorkingDaysBusiness, int> _memberWorkingDaysRepository;

        public ManageMemberWorkingDaysService(IRepository<MemberWorkingDaysBusiness, int> memberWorkingDaysRepository)
        {
            _memberWorkingDaysRepository = memberWorkingDaysRepository;
        }

        public async Task<MemberWorkingDaysBusiness> GetWorkingDaysByIdAsync(int workingDaysId)
        {
            var workingDaysEntity = await _memberWorkingDaysRepository.GetAllAsync();
            var workingDays = workingDaysEntity.FirstOrDefault(x => x.Id == workingDaysId);
            return workingDays;
        }

        public async Task<bool> AddMemberWorkingDaysAsync(MemberWorkingDaysBusiness memberWorkingDays)
        {
            if (memberWorkingDays.WorkingDays < 0) return false;
            return await _memberWorkingDaysRepository.InsertAsync(memberWorkingDays);
        }

        public async Task<bool> EditMemberWorkingDaysAsync(MemberWorkingDaysBusiness memberWorkingDays)
        {
            var oldMemberWorkingDays = await _memberWorkingDaysRepository.GetByIdAsync(memberWorkingDays.Id);

            if (oldMemberWorkingDays == null ||
                memberWorkingDays.WorkingDays < 0)
            {
                return false;
            }

            return await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);
        }

        public async Task<IEnumerable<MemberWorkingDaysBusiness>> GetAllWorkingDaysForSprintAsync(int sprintId)
        {
            var workingDaysEntity = await _memberWorkingDaysRepository.GetAllAsync();
            var allWorkingDaysForSprint = workingDaysEntity.Where(x => x.SprintId == sprintId);
            return allWorkingDaysForSprint;
        }
    }
}
