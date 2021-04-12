using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Repository;

namespace Teams.Business.Services
{
    public class ManageMemberWorkingDaysService : IManageMemberWorkingDaysService
    {
        private readonly IRepository<Data.Models.MemberWorkingDays, Business.Models.MemberWorkingDays, int> _memberWorkingDaysRepository;

        public ManageMemberWorkingDaysService(IRepository<Data.Models.MemberWorkingDays, Business.Models.MemberWorkingDays, int> memberWorkingDaysRepository)
        {
            _memberWorkingDaysRepository = memberWorkingDaysRepository;
        }

        public async Task<Business.Models.MemberWorkingDays> GetWorkingDaysByIdAsync(int workingDaysId)
        {
            var workingDaysEntity = await _memberWorkingDaysRepository.GetAllAsync();
            var workingDays = workingDaysEntity.FirstOrDefault(x => x.Id == workingDaysId);
            return workingDays;
        }

        public async Task<bool> AddMemberWorkingDaysAsync(Business.Models.MemberWorkingDays memberWorkingDays)
        {
            if (memberWorkingDays.WorkingDays < 0) return false;
            return await _memberWorkingDaysRepository.InsertAsync(memberWorkingDays);
        }

        public async Task<bool> EditMemberWorkingDaysAsync(Business.Models.MemberWorkingDays memberWorkingDays)
        {
            var oldMemberWorkingDays = await _memberWorkingDaysRepository.GetByIdAsync(memberWorkingDays.Id);

            if (oldMemberWorkingDays == null ||
                memberWorkingDays.WorkingDays < 0)
            {
                return false;
            }

            return await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);
        }

        public async Task<IEnumerable<Business.Models.MemberWorkingDays>> GetAllWorkingDaysForSprintAsync(int sprintId)
        {
            var workingDaysEntity = await _memberWorkingDaysRepository.GetAllAsync();
            var allWorkingDaysForSprint = workingDaysEntity.Where(x => x.SprintId == sprintId);
            return allWorkingDaysForSprint;
        }
    }
}
