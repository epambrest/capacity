using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Teams.Security;
using Teams.Services;
using System.Security.Authentication;
using NUnit.Framework;
using Teams.Data;
using Teams.Models;
using Microsoft.EntityFrameworkCore;
using Teams.Data.Migrations;
using System.Collections.Generic;
using System.Linq;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private IManageTeamsService _mangeTeamsService;
        private Mock<ICurrentUser> _currentUser;
        private Mock<ApplicationDbContext> _db;
        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _db = new Mock<ApplicationDbContext>();
            
        }
        [Test]
        public void GetMyTeams_ManageTeamsServiceReturnsListCount4_ListCount4()
        {
            //Arrange
            string user = "abc-def";
            List<Models.Team> myteams = new List<Models.Team>
            {
                 new Models.Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1"},
                 new Models.Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4"},
                 new Models.Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2"},
                 new Models.Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9"},
            };
            _currentUser.Setup(x => x.Current.Id()).Returns(user);
            _db.Setup(x => x.Team.ToList()).Returns(GetTestTeams());
            _db.Setup(x => x.TeamMembers.ToArray()).Returns(GetTestTeamMembers());

            //Act
            List<Models.Team> result = new List<Models.Team>( _mangeTeamsService.GetMyTeams());

            //Assert
            Assert.AreEqual(result, myteams);

        }
        private List<Models.Team> GetTestTeams()
        {
            List<Models.Team> teams = new List<Models.Team>
            {
                new Models.Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1"},
                new Models.Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2"},
                new Models.Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3"},
                new Models.Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4"},
                new Models.Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5"},
                new Models.Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6"},
                new Models.Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7"},
                new Models.Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8"},
                new Models.Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9"},
                new Models.Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10"}
            };
            return teams;
        }
        private TeamMember[] GetTestTeamMembers()
        {
            TeamMember[] teammembers = new TeamMember[] {

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

            return teammembers;
        }
    }
}
