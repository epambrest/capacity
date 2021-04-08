using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

namespace Teams.Data.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserBusiness, User>().ReverseMap();
        }
    }
}
