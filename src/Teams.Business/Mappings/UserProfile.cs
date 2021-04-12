using AutoMapper;
using Teams.Business.Models;
using Teams.Data.Models;

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
