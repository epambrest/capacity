using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskBusiness, Data.Models.Task>().ReverseMap();
        }
    }
}
