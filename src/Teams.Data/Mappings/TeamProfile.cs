using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Data.Mappings
{
    public class TeamProfile : Profile 
    {
        public TeamProfile()
        {
            CreateMap<TeamBusiness, Team>().ReverseMap();
        }
    }
}
