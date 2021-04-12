using AutoMapper;

namespace Teams.Business.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Business.Models.User, Data.Models.User>().ReverseMap();
        }
    }
}
