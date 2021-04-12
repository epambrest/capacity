using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Business.Mappings
{
    public class TeamProfile : Profile 
    {
        public TeamProfile()
        {
            CreateMap<Business.Models.Team, Data.Models.Team>().ReverseMap();
        }
    }
}
