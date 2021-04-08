using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Data.Mappings
{
    public class SprintProfile : Profile
    {
        public SprintProfile()
        {
            CreateMap<SprintBusiness, Sprint>().ReverseMap();
        }
    }
}
