using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Business.Mappings
{
    public class SprintProfile : Profile
    {
        public SprintProfile()
        {
            CreateMap<Business.Models.Sprint, Data.Models.Sprint>().ReverseMap();
        }
    }
}
