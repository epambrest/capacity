using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Business.Mappings
{
    public class TeamMemberProfile : Profile
        {
            public TeamMemberProfile()
            {
                CreateMap<Business.Models.TeamMember, Data.Models.TeamMember>().ReverseMap();
            }
        }
}
