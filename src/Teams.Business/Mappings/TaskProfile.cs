using AutoMapper;
using Teams.Business.Models;

namespace Teams.Business.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, Data.Models.Task>().ReverseMap();
        }
    }
}
