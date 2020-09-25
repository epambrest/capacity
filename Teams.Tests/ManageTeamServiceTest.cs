using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Microsoft.EntityFrameworkCore;
using Teams.Security;
using Microsoft.AspNetCore.Http;
using Moq;
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
            public async Task Remove_TeamRepositoryReturnsTrue_ReturnsTrue()
            {

                // Arrange
                string team_owner = "1234";
                int team_id = 1;
                var teams = new List<Team>
            {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                 new Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"},
                 new Team { Id= 3, TeamOwner = "4152", TeamName = "Third_Team"},
            }
                .AsQueryable();


                _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
             .Returns(new Claim("UserName", team_owner));
                _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
                _teamRepository.Setup(x => x.GetAll()).Returns(teams);

                //Act
                var result = await _manageTeamsService.Remove(team_id);

                //Assert
                Assert.IsTrue(result);
            }

        }
    }
}




