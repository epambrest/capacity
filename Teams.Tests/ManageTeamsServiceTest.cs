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

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private ManageTeamsService manageTeamsService;
        public Mock<TeamRepository> teamRepository;
        public Mock<CurrentUser> currentUser;
        private Mock<HttpContextAccessor> httpContextAccessor;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName:"TeamCapacity").Options;
            teamRepository = new Mock<TeamRepository>();
            httpContextAccessor = new Mock<HttpContextAccessor>();
            currentUser = new Mock<CurrentUser>();
            manageTeamsService = new ManageTeamsService(currentUser.Object, teamRepository.Object);
        }
        [Test]
        public void AddTeamWithEmptyName()
        {
            //Arrange
            httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>())).Returns(new Claim("name", "abc-def"));
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            string teamName = "";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
        }
        [Test]
        public void AddTeamWithIllegalName()
        {
            //Arrange
            string teamName = "alex wins:";
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.Name).Returns("Max");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(false, isValid);
        }
        [Test]
        public void AddTeamWithLegalName()
        {
            //Arrange
            string teamName = "Juventus_FC";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(true, isValid);
        }
        public void AddTeamWithExistingName()
        {
            //Arrange
            string teamName = "Juventus_FC";
            //Act
            bool isValid = manageTeamsService.AddTeamAsync(teamName).Result;
            //Assert
            Assert.AreEqual(true, isValid);
        }
    }
}
