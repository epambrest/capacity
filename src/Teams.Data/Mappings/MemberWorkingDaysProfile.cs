using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Data.Mappings
{
    public class MemberWorkingDaysProfile : Profile
    {
        public MemberWorkingDaysProfile()
        {
            CreateMap<MemberWorkingDaysBusiness, MemberWorkingDays>().ReverseMap();
        }
    }
}
