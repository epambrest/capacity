using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class TeamMemberProfile : Profile
    {
        public TeamMemberProfile()
        {
            CreateMap<Models.TeamMember, TeamMember>().ReverseMap();
        }
    }
}
