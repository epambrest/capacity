using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile() => CreateMap<Models.User, User>().ReverseMap();
    }
}
