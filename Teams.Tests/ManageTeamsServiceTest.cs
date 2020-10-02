﻿using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        public IManageTeamsService _manageTeamsService;

        public Mock<IRepository<Team, int>> _teamRepository;

        public Mock<ICurrentUser> _currentUser;
        
        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<Team, int>>();

            _currentUser = new Mock<ICurrentUser>();

            _manageTeamsService = new ManageTeamsService(_currentUser.Object, _teamRepository.Object);
            
        }

        [Test]
        public async Task GetMyTeamsAsync_ManageTeamsServiceReturnsListCount4_ListCount4()
        {
            //Arrange
            const string id = "abc-def";
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="def-abc", TeamId =1}}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =2}}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =4}}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =9}}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}}
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = new List<Team>(await _manageTeamsService.GetMyTeamsAsync());

            //Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(4, result[1].Id);
            Assert.AreEqual(2, result[2].Id);
            Assert.AreEqual(9, result[3].Id);
        }
    }
}
