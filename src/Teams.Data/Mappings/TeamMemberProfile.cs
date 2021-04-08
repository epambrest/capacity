using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Data.Mappings
{
    public class TeamMemberProfile : Profile
        {
            public TeamMemberProfile()
            {
                CreateMap<TeamMemberBusiness, TeamMember>().ReverseMap();
            }
        }
}
