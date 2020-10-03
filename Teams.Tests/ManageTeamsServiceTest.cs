using Moq;
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
using Teams.Repository;
using System;
using System.Threading.Tasks;
using Teams.Services;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private ManageTeamsService manageTeamsService;
        private Mock <IRepository<Team, int>> testTeamRepo;
        private Mock<ICurrentUser> currentUserMock;

        [SetUp]
        public void Setup()
        {
            currentUserMock =  new Mock<ICurrentUser>();
            testTeamRepo = new Mock<IRepository<Team, int>>();
            string ownerId = Guid.NewGuid().ToString();
            var userDetails = new Mock<UserDetails>(null);
            userDetails.Setup(x => x.Id()).Returns(ownerId);
            currentUserMock.SetupGet(x => x.Current).Returns(userDetails.Object);
            manageTeamsService = new ManageTeamsService(currentUserMock.Object, testTeamRepo.Object);
        }
        [Test]
        public void AddTeamAsync_AddTeamWithEmptyName_ReturnsFalse()
        {
            //Arrange
            string teamName = "";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
        }
        [Test]
        public void AddTeamAsync_AddTeamWithIllegalName_ReturnsFalse()
        {
            //Arrange
            string teamName = "alex wins:";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
        }
        [Test]
        public void AddTeamAsync_AddTeamWithLegalName_ReturnsTrue()
        {
            //Arrange
            string teamName = "Legal_Team.";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(true, isValid);
        }
        [Test]
        public async Task AddTeamAsync_AddTeamWithExistingName_ReturnsFalseAsync()
        {
            //Arrange
            string teamName = "Su_per-Team.,";
            await manageTeamsService.AddTeamAsync(teamName);
            var existingTeams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = teamName},

            }.AsQueryable();
            testTeamRepo.Setup(x => x.GetAll()).Returns(existingTeams);
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
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

            testTeamRepo.Setup(x => x.GetAll()).Returns(teams.AsQueryable());
            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = new List<Team>(manageTeamsService.GetMyTeams());

            //Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(4, result[1].Id);
            Assert.AreEqual(2, result[2].Id);
            Assert.AreEqual(9, result[3].Id);
        }
    }
}
