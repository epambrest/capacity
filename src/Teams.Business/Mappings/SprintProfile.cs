using AutoMapper;

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
