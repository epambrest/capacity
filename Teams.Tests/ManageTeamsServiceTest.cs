using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        public IManageTeamsService _mangeTeamsService;

        public Mock<IRepository<Team, int>> _teamRepository;

        public Mock<ICurrentUser> _currentUser;
        
        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<Team, int>>();

            _currentUser = new Mock<ICurrentUser>();

            _mangeTeamsService = new ManageTeamsService(_currentUser.Object, _teamRepository.Object);
            
        }

        [Test]
        public void GetMyTeams_ManageTeamsServiceReturnsListCount4_ListCount4()
        {
            //Arrange
            string id = "abc-def";
            var myteams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1"},
                 new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4"},
                 new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2"},
                 new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9"},
                 
            };

            _teamRepository.Setup(x => x.GetAll()).Returns(GetTestTeams());
            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);


            //Act
            var result = new List<Team>(_mangeTeamsService.GetMyTeams());

            //Assert
            Assert.AreEqual(myteams.Count(), result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(4, result[1].Id);
            Assert.AreEqual(2, result[2].Id);
            Assert.AreEqual(9, result[3].Id);
        }

        private IQueryable<Team> GetTestTeams()
        {
            var members1 = new List<TeamMember>{new TeamMember {MemberId="def-abc", TeamId =1}};

            var members2 = new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =2}};

            var members4 = new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =4}};

            var members9 = new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =9}};

            var members7 = new List<TeamMember>{new TeamMember {MemberId="abc-defa", TeamId =7}};
            
            var samemember = new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}};
            
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=members1},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=members2},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=samemember},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=members4},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=samemember},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=samemember},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=members7},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=samemember},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=members9},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=samemember}
            };

            return teams.AsQueryable();
        }
    }
}
