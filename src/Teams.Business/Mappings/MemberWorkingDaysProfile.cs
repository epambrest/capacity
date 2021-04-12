using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Business.Mappings
{
    public class MemberWorkingDaysProfile : Profile
    {
        public MemberWorkingDaysProfile()
        {
            CreateMap<Business.Models.MemberWorkingDays, Data.Models.MemberWorkingDays>().ReverseMap();
        }
    }
}
