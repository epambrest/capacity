using Moq;
using Teams.Security;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;
using Teams.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private ManageTeamsService _manageTeamsService;
        private Mock <IRepository<Team, int>> _testTeamRepo;
        private Mock<ICurrentUser> _currentUserMock;

        [SetUp]
        public void Setup()
        {
            _currentUserMock =  new Mock<ICurrentUser>();
            _testTeamRepo = new Mock<IRepository<Team, int>>();
            string ownerId = Guid.NewGuid().ToString();
            var userDetails = new Mock<UserDetails>(null);
            userDetails.Setup(x => x.Id()).Returns(ownerId);
            _currentUserMock.SetupGet(x => x.Current).Returns(userDetails.Object);
            _manageTeamsService = new ManageTeamsService(_currentUserMock.Object, _testTeamRepo.Object);
        }

        [Test]
        public async Task AddTeamAsync_AddTeamWithEmptyName_ReturnsFalse()
        {
            //Arrange
            string teamName = "";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task AddTeamAsync_AddTeamWithIllegalName_ReturnsFalse()
        {
            //Arrange
            string teamName = "alex wins:";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task AddTeamAsync_AddTeamWithLegalName_ReturnsTrue()
        {
            //Arrange
            string teamName = "Legal_Team.";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.AreEqual(isValid, Is.True);
        }

        [Test]
        public async Task AddTeamAsync_AddTeamWithExistingName_ReturnsFalseAsync()
        {
            //Arrange
            string teamName = "Su_per-Team.,";
            await _manageTeamsService.AddTeamAsync(teamName);
            var existingTeams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = teamName},

            }.AsQueryable();
            _testTeamRepo.Setup(x => x.GetAll()).Returns(existingTeams);
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.False);
        }
        
        [Test]
        public void GetMyTeams_ManageTeamsServiceReturnsListCount4_ListCount4()
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

            _testTeamRepo.Setup(x => x.GetAll()).Returns(teams.AsQueryable());
            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = new List<Team>(_manageTeamsService.GetMyTeams());

            //Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(4, result[1].Id);
            Assert.AreEqual(2, result[2].Id);
            Assert.AreEqual(9, result[3].Id);
        }

        [Test]
        public async Task GetTeamAsync_ManageTeamsServiceReturnsTeam_Team()
        {
            //Arrange
            const string id = "abc-def";
            const int team_id = 3;
            Team team = new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}};

            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(team);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = await _manageTeamsService.GetTeamAsync(team_id);

            //Assert
            Assert.AreEqual(3, result.Id);
        }
    }
}
