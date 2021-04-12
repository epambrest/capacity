using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class MemberWorkingDaysProfile : Profile
    {
        public MemberWorkingDaysProfile()
        {
            CreateMap<MemberWorkingDays, Models.MemberWorkingDays>().ReverseMap();
        }
    }
}
