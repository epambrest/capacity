using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, Models.Task>().ReverseMap();
        }
    }
}
