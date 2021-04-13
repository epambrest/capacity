﻿using AutoMapper;
using Teams.Business.Models;

namespace Teams.Data.Mappings
{
    public class TeamProfile : Profile 
    {
        public TeamProfile()
        {
            CreateMap<Team, Models.Team>().ReverseMap();
        }
    }
}
