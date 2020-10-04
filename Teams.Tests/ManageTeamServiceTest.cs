using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Microsoft.AspNetCore.Http;
using Moq;
using MockQueryable.Moq;
using System.Security.Claims;
using Teams.Services;
using System.Threading.Tasks;

namespace Teams.Tests
{
    class ManageTeamServiceTest
    {
        [TestFixture]
        public class CurrentUserTest
        {
            private Mock<IHttpContextAccessor> _httpContextAccessor;
            private Mock<IRepository<Team, int>> _teamRepository;
            private ICurrentUser _currentUser;
            private ManageTeamsService _manageTeamsService;

            [SetUp]
            public void Setup()
            {
                _httpContextAccessor = new Mock<IHttpContextAccessor>();
                _currentUser = new CurrentUser(_httpContextAccessor.Object);
                _teamRepository = new Mock<IRepository<Team, int>>();
                _manageTeamsService = new ManageTeamsService(_currentUser, _teamRepository.Object);
            }

            [Test]
            public async Task RemoveAsync_ManageTeamsServiceReturnsTrue_ReturnsTrue()
            {
                // Arrange
                string team_owner = "1234";
                int team_id = 1;
                var teams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                 new Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"}
            }.AsQueryable();

                _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
             .Returns(new Claim("UserName", team_owner));
                _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .ReturnsAsync(true);

                //Act
                var result = await _manageTeamsService.RemoveAsync(team_id);
              
                //Assert
                Assert.IsTrue(result); 
            }

            [Test]
            public async Task RemoveAsync_ManageTeamsServiceReturnsFalse_ReturnsFalse()
            {
                // Arrange
                string team_owner = "1234";
                int team_id1 = 4;
                int team_id2 = 3;
                var teams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                 new Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"},
                 new Team { Id= 3, TeamOwner = "4152", TeamName = "Third_Team"},
            }.AsQueryable();

                _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
             .Returns(new Claim("UserName", team_owner));
                _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);


                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .ReturnsAsync(true);

                //Act
                var result1 = await _manageTeamsService.RemoveAsync(team_id1);
                var result2 = await _manageTeamsService.RemoveAsync(team_id2);

                //Assert
                Assert.IsFalse(result1);
                Assert.IsFalse(result2);
            }

        }
    }
}




