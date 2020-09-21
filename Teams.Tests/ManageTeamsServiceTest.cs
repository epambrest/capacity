﻿using Moq;
using Teams.Security;
using NUnit.Framework;
using Teams.Data;
using Teams.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Teams.Services;
using Microsoft.EntityFrameworkCore;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        public IManageTeamsService _mangeTeamsService;
        public ICurrentUser _currentUser;
        public Mock<IRepository<Team, int>> _teamRepository;
        public Mock<IRepository<TeamMember, int>> _teamMemberRepository;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        [SetUp]
        public void Setup()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _teamRepository = new Mock<IRepository<Team, int>>();
            _teamMemberRepository = new Mock<IRepository<TeamMember, int>>();
            _currentUser = new  CurrentUser(_httpContextAccessor.Object);
            _mangeTeamsService = new ManageTeamsService(_currentUser, _teamRepository.Object, _teamMemberRepository.Object);
        }
        [Test]
        public void GetMyTeams_ManageTeamsServiceReturnsListCount4_ListCount4()
        {
            //Arrange
            string id = "abc-def";
            List<Team> myteams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1"},
                 new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4"},
                 new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2"},
                 new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9"},
            };
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("name", id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            _teamRepository.Setup(x => x.GetAll()).Returns(GetTestTeams());
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(GetTestTeamMembers());

            //Act
            List<Team> result = new List<Team>(_mangeTeamsService.GetMyTeams());

            //Assert
            bool compare = true;
            for(int i =0; i< result.Count(); i++)
            {
                if (myteams[i].Id != result[i].Id) compare = false;
            }
            Assert.IsTrue(compare);

        }
        private IQueryable<Team> GetTestTeams()
        {
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1"},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2"},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3"},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4"},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5"},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6"},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7"},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8"},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9"},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10"}
            };
            return teams.AsQueryable();
        }
        private IQueryable<TeamMember> GetTestTeamMembers()
        {
            var teammembers = new List<TeamMember> 
            {

            new TeamMember { Id = 1, TeamId = 1, MemberId = "def-abc" },
            new TeamMember { Id = 2, TeamId = 2, MemberId = "abc-def" },
            new TeamMember { Id = 3, TeamId = 3, MemberId = "zxc-cxz" },
            new TeamMember { Id = 4, TeamId = 4, MemberId = "def-abc" },
            new TeamMember { Id = 5, TeamId = 5, MemberId = "cxz-zxc" },
            new TeamMember { Id = 6, TeamId = 6, MemberId = "zxc-cxz" },
            new TeamMember { Id = 7, TeamId = 7, MemberId = "cxz-zxc" },
            new TeamMember { Id = 8, TeamId = 8, MemberId = "zxc-cxz" },
            new TeamMember { Id = 9, TeamId = 9, MemberId = "abc-def" },
            new TeamMember { Id = 10, TeamId = 10, MemberId = "cxz-zxc" }
        };

            return teammembers.AsQueryable();
        }
    }
}