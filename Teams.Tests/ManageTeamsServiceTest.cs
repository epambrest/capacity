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

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private ManageTeamsService manageTeamsService;
        private IRepository<Team, int> testTeamRepo;
        private Mock<ICurrentUser> currentUserMock;
        ApplicationDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            currentUserMock =  new Mock<ICurrentUser>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDB").Options;
            dbContext = new ApplicationDbContext(options);
            testTeamRepo = new TeamRepository(dbContext);
            string ownerId = Guid.NewGuid().ToString();
            var userDetails = new Mock<UserDetails>(null);
            userDetails.Setup(x => x.Id()).Returns(ownerId);
            currentUserMock.SetupGet(x => x.Current).Returns(userDetails.Object);
            manageTeamsService = new ManageTeamsService(currentUserMock.Object, testTeamRepo);
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
            //Act
            await manageTeamsService.AddTeamAsync(teamName);
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
        }
    }
}
