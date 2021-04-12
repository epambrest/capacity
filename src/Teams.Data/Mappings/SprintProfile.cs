using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class SprintProfile : Profile
    {
        public SprintProfile()
        {
            CreateMap<Sprint, Models.Sprint>().ReverseMap();
        }
    }
}
